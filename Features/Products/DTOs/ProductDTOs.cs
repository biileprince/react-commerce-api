namespace ReactCommerce.Api.Features.Products.DTOs;

public record ProductResponse(
    Guid Id, string Name, string Slug, string Description,
    decimal Price, string Currency, int StockQuantity,
    string CategorySlug, string CategoryName,
    List<string> Images, decimal Rating, int ReviewCount,
    bool IsFeatured, List<string> Tags, DateTime CreatedAt);

public record ProductQueryParams(
    string? Search, string? Category,
    decimal? MinPrice, decimal? MaxPrice,
    string? Sort, int Page = 1, int Limit = 20);
