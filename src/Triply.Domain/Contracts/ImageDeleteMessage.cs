namespace Triply.Domain.Contracts;

/// <summary>Message payload published for the image delete.</summary>
public class ImageDeleteMessage
{
    /// <summary>Gets or sets the identifier of the image.</summary>
    public Guid ImageId { get; set; }
    /// <summary>Gets or sets the identifier of the hotel.</summary>
    public Guid HotelId { get; set; }
    /// <summary>Gets or sets the identifier of the public.</summary>
    public string PublicId { get; set; } = null!;
}
