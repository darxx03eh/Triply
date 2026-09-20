using System.Text.Json.Serialization;

namespace Triply.Application.Features.Cart.Commands.AddCartItem;

/// <summary>Data required to add to cart.</summary>
public class AddCartItemRequest
{
    /// <summary>Gets or sets the identifier of the user.</summary>
    [JsonIgnore]
    public Guid UserId { get; set; }
    /// <summary>Gets or sets the identifier of the room.</summary>
    public Guid RoomId { get; set; }
    /// <summary>Gets or sets the check in.</summary>
    public DateOnly CheckIn { get; set; }
    /// <summary>Gets or sets the check-out.</summary>
    public DateOnly CheckOut { get; set; }
    /// <summary>Gets or sets the  adults numbers.</summary>
    public short Adults { get; set; } = 2;
    /// <summary>Gets or sets the children numbers.</summary>
    public short Children { get; set; }
}