using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Products;

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

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products").WithTags("Products");

        group.MapGet("/", GetProducts);
        group.MapGet("/featured", GetFeatured);
        group.MapGet("/{slug}", GetBySlug);
        group.MapGet("/{id:guid}/related", GetRelated);
    }

    private static async Task<IResult> GetProducts([AsParameters] ProductQueryParams query, AppDbContext db)
    {
        var q = db.Products
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Tags)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.ToLower().Trim();
            q = q.Where(p =>
                p.Name.ToLower().Contains(search) ||
                p.Description.ToLower().Contains(search) ||
                p.Tags.Any(t => t.Tag.Contains(search)) ||
                p.Category.Name.ToLower().Contains(search));
        }

        if (!string.IsNullOrWhiteSpace(query.Category))
            q = q.Where(p => p.Category.Slug == query.Category);

        if (query.MinPrice.HasValue)
            q = q.Where(p => p.Price >= query.MinPrice.Value);

        if (query.MaxPrice.HasValue)
            q = q.Where(p => p.Price <= query.MaxPrice.Value);

        q = query.Sort switch
        {
            "price_asc" => q.OrderBy(p => p.Price),
            "price_desc" => q.OrderByDescending(p => p.Price),
            "rating" => q.OrderByDescending(p => p.Rating),
            _ => q.OrderByDescending(p => p.CreatedAt),
        };

        var total = await q.CountAsync();
        var page = Math.Max(1, query.Page);
        var limit = Math.Clamp(query.Limit, 1, 100);
        var totalPages = (int)Math.Ceiling((double)total / limit);

        var products = await q
            .Skip((page - 1) * limit)
            .Take(limit)
            .ToListAsync();

        return Results.Ok(new PaginatedResponse<ProductResponse>
        {
            Data = products.Select(ToDto).ToList(),
            Meta = new PaginationMeta
            {
                Page = page,
                Limit = limit,
                Total = total,
                TotalPages = totalPages,
            }
        });
    }

    private static async Task<IResult> GetFeatured(AppDbContext db, int limit = 6)
    {
        var products = await db.Products
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Tags)
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(Math.Clamp(limit, 1, 20))
            .ToListAsync();

        return Results.Ok(ApiResponse<List<ProductResponse>>.Ok(products.Select(ToDto).ToList()));
    }

    private static async Task<IResult> GetBySlug(string slug, AppDbContext db)
    {
        var product = await db.Products
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (product is null) return Results.NotFound(ApiResponse<object>.Fail("Product not found"));

        return Results.Ok(ApiResponse<ProductResponse>.Ok(ToDto(product)));
    }

    private static async Task<IResult> GetRelated(Guid id, AppDbContext db, int limit = 4)
    {
        var product = await db.Products.FindAsync(id);
        if (product is null) return Results.NotFound();

        var related = await db.Products
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Tags)
            .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
            .OrderByDescending(p => p.Rating)
            .Take(Math.Clamp(limit, 1, 10))
            .ToListAsync();

        return Results.Ok(ApiResponse<List<ProductResponse>>.Ok(related.Select(ToDto).ToList()));
    }

    private static ProductResponse ToDto(Entities.Product p) => new(
        p.Id, p.Name, p.Slug, p.Description,
        p.Price, p.Currency, p.StockQuantity,
        p.Category.Slug, p.Category.Name,
        p.Images.Select(i => i.Url).ToList(),
        p.Rating, p.ReviewCount, p.IsFeatured,
        p.Tags.Select(t => t.Tag).ToList(),
        p.CreatedAt);
}
