namespace ReactCommerce.Api.Entities;

public class ProductTag
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public required string Tag { get; set; }

    public Product Product { get; set; } = null!;
}
