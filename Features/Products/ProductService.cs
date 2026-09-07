using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Features.Products.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Products;

public class ProductService : IProductService
{
    private readonly AppDbContext _db;

    public ProductService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<PaginatedResponse<ProductResponse>> GetProductsAsync(ProductQueryParams query)
    {
        var q = _db.Products
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

        return new PaginatedResponse<ProductResponse>
        {
            Data = products.Select(ToDto).ToList(),
            Meta = new PaginationMeta
            {
                Page = page,
                Limit = limit,
                Total = total,
                TotalPages = totalPages,
            }
        };
    }

    public async Task<List<ProductResponse>> GetFeaturedAsync(int limit = 6)
    {
        var products = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Tags)
            .Where(p => p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(Math.Clamp(limit, 1, 20))
            .ToListAsync();

        return products.Select(ToDto).ToList();
    }

    public async Task<ProductResponse?> GetBySlugAsync(string slug)
    {
        var product = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Tags)
            .FirstOrDefaultAsync(p => p.Slug == slug);

        if (product is null) return null;

        return ToDto(product);
    }

    public async Task<List<ProductResponse>> GetRelatedAsync(Guid id, int limit = 4)
    {
        var product = await _db.Products.FindAsync(id);
        if (product is null) return new List<ProductResponse>();

        var related = await _db.Products
            .Include(p => p.Category)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Tags)
            .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
            .OrderByDescending(p => p.Rating)
            .Take(Math.Clamp(limit, 1, 10))
            .ToListAsync();

        return related.Select(ToDto).ToList();
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
