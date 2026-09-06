using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Categories;

public record CategoryResponse(Guid Id, string Name, string Slug, string Description, string? Icon, bool IsActive, int ProductCount);

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetAll);
    }

    private static async Task<IResult> GetAll(AppDbContext db)
    {
        var categories = await db.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryResponse(
                c.Id, c.Name, c.Slug, c.Description, c.Icon, c.IsActive,
                c.Products.Count(p => !p.IsDeleted)))
            .ToListAsync();

        return Results.Ok(ApiResponse<List<CategoryResponse>>.Ok(categories));
    }
}
