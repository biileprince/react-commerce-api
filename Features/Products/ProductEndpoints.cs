using ReactCommerce.Api.Features.Products.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Products;

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

    private static async Task<IResult> GetProducts([AsParameters] ProductQueryParams query, IProductService productService)
    {
        var response = await productService.GetProductsAsync(query);
        return Results.Ok(response);
    }

    private static async Task<IResult> GetFeatured(IProductService productService, int limit = 6)
    {
        var products = await productService.GetFeaturedAsync(limit);
        return Results.Ok(ApiResponse<List<ProductResponse>>.Ok(products));
    }

    private static async Task<IResult> GetBySlug(string slug, IProductService productService)
    {
        var product = await productService.GetBySlugAsync(slug);
        if (product is null) return Results.NotFound(ApiResponse<object>.Fail("Product not found"));

        return Results.Ok(ApiResponse<ProductResponse>.Ok(product));
    }

    private static async Task<IResult> GetRelated(Guid id, IProductService productService, int limit = 4)
    {
        var related = await productService.GetRelatedAsync(id, limit);
        if (!related.Any()) return Results.NotFound();

        return Results.Ok(ApiResponse<List<ProductResponse>>.Ok(related));
    }
}
