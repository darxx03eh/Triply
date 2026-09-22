using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Triply.Domain.Entities;

namespace Triply.Infrastructure.Documents;

/// <summary>Renders a PDF invoice for one or more bookings sharing the same confirmation number.</summary>
/// <param name="bookings">The bookings to include on the invoice.</param>
/// <param name="currency">The ISO 4217 currency code used to format monetary amounts.</param>
public class BookingInvoiceDocument(IReadOnlyList<Booking> bookings, string currency) : IDocument
{
    private const string Primary = "#18342F";
    private const string Muted = "#5B706B";
    private const string Border = "#DCE8E3";

    private readonly Booking _first = bookings[0];

    /// <summary>Composes the invoice page layout.</summary>
    /// <param name="container">The document container to render into.</param>
    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.Margin(40);
            page.DefaultTextStyle(x => x.FontSize(10).FontColor(Primary));

            page.Header().Element(ComposeHeader);
            page.Content().PaddingVertical(20).Element(ComposeContent);
            page.Footer().AlignCenter().Text(text =>
            {
                text.Span("Thank you for booking with Triply. Page ").FontColor(Muted);
                text.CurrentPageNumber().FontColor(Muted);
            });
        });
    }

    /// <summary>Composes the header section, containing the brand block and invoice metadata.</summary>
    /// <param name="container">The container to render the header into.</param>
    private void ComposeHeader(IContainer container)
    {
        container.Row(row =>
        {
            row.RelativeItem().Column(column =>
            {
                column.Item().Text("TRIPLY").FontSize(22).Bold();
                column.Item().Text("Booking invoice").FontColor(Muted);
            });

            row.RelativeItem().AlignRight().Column(column =>
            {
                column.Item().Text($"Invoice #{_first.ConfirmationNumber}").Bold();
                column.Item().Text($"Issued: {DateTime.UtcNow:dd MMM yyyy}").FontColor(Muted);
                column.Item().Text($"Paid: {_first.Payment?.PaidAt:dd MMM yyyy}").FontColor(Muted);
            });
        });
    }

    /// <summary>Composes the body section, containing the guest details, line items table, and totals.</summary>
    /// <param name="container">The container to render the content into.</param>
    private void ComposeContent(IContainer container)
    {
        container.Column(column =>
        {
            column.Spacing(16);

            column.Item().Column(guest =>
            {
                guest.Item().Text("Billed to").Bold();
                guest.Item().Text(_first.GuestFullName);
                guest.Item().Text(_first.GuestEmail).FontColor(Muted);
                if (_first.GuestPhoneNumber is not null)
                    guest.Item().Text(_first.GuestPhoneNumber).FontColor(Muted);
            });

            column.Item().Table(table =>
            {
                table.ColumnsDefinition(columns =>
                {
                    columns.RelativeColumn(3);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(1);
                    columns.RelativeColumn(2);
                    columns.RelativeColumn(2);
                });

                table.Header(header =>
                {
                    foreach (var title in new[] { "Hotel", "Room", "Dates", "Nights", "Discount", "Total" })
                        header.Cell().BorderBottom(1).BorderColor(Border).PaddingVertical(6).Text(title).Bold();
                });

                foreach (var booking in bookings)
                {
                    var nights = (booking.CheckOut.Date - booking.CheckIn.Date).Days;
                    table.Cell().Element(Cell).Text($"{booking.Room.Hotel.Name}\n{booking.Room.Hotel.City.Name}");
                    table.Cell().Element(Cell).Text($"{booking.Room.RoomType} #{booking.Room.Number}");
                    table.Cell().Element(Cell).Text($"{booking.CheckIn:dd MMM} - {booking.CheckOut:dd MMM yyyy}");
                    table.Cell().Element(Cell).Text(nights.ToString());
                    table.Cell().Element(Cell).Text(Money(booking.DiscountAmount));
                    table.Cell().Element(Cell).Text(Money(booking.TotalPrice));
                }
            });

            column.Item().AlignRight().Column(total =>
            {
                total.Item().Text($"Discount: {Money(bookings.Sum(b => b.DiscountAmount))}").FontColor(Muted);
                total.Item().Text($"Total paid: {Money(bookings.Sum(b => b.TotalPrice))}").FontSize(14).Bold();
            });

            if (_first.SpecialRequests is not null)
                column.Item().Column(requests =>
                {
                    requests.Item().Text("Special requests").Bold();
                    requests.Item().Text(_first.SpecialRequests).FontColor(Muted);
                });
        });
    }

    /// <summary>Applies the shared cell styling (bottom border and vertical padding) to a table cell.</summary>
    /// <param name="container">The cell container to style.</param>
    /// <returns>The styled container.</returns>
    private static IContainer Cell(IContainer container)
        => container.BorderBottom(1).BorderColor(Border).PaddingVertical(6);

    /// <summary>Formats a monetary amount with the invoice's currency code.</summary>
    /// <param name="amount">The amount to format.</param>
    /// <returns>The formatted amount, e.g. "120.00 USD".</returns>
    private string Money(decimal amount) => $"{amount:N2} {currency.ToUpper()}";
}