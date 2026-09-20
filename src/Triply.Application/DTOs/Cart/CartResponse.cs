namespace Triply.Application.DTOs.Cart;

public record CartResponse
{
    /// <summary>Gets the items.</summary>
    public IReadOnlyList<CartItemResponse> Items { get; init; } = [];
    /// <summary>Gets the items count.</summary>
    public int ItemsCount { get; init; }
    /// <summary>Gets the original price.</summary>
    public decimal OriginalPrice { get; init; }
    /// <summary>Gets the discount amount.</summary>
    public decimal DiscountAmount { get; init; }
    /// <summary>Gets the total price.</summary>
    public decimal TotalPrice { get; init; }
}