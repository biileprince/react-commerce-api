namespace ReactCommerce.Api.Entities;

public class ProductImage
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public required string Url { get; set; }
    public int SortOrder { get; set; }

    public Product Product { get; set; } = null!;
}
