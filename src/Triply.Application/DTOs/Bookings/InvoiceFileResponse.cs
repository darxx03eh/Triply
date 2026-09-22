namespace Triply.Application.DTOs.Bookings;

/// <summary>Represents a generated invoice file ready to be returned to the client.</summary>
/// <param name="Content">The raw bytes of the invoice file.</param>
/// <param name="FileName">The suggested file name for the downloaded invoice.</param>
public record InvoiceFileResponse(byte[] Content, string FileName);