using ReactCommerce.Api.Features.Categories.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Categories;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/categories").WithTags("Categories");

        group.MapGet("/", GetAll);
    }

    private static async Task<IResult> GetAll(ICategoryService categoryService)
    {
        var categories = await categoryService.GetAllActiveAsync();
        return Results.Ok(ApiResponse<List<CategoryResponse>>.Ok(categories));
    }
}
