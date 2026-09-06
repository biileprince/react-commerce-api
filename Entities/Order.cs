namespace ReactCommerce.Api.Entities;

public class Order
{
    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid ShippingAddressId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal ShippingFee { get; set; }
    public decimal Total { get; set; }
    public string Currency { get; set; } = "GHS";
    public string Status { get; set; } = "pending";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    public User? User { get; set; }
    public Address ShippingAddress { get; set; } = null!;
    public ICollection<OrderItem> Items { get; set; } = [];
}
