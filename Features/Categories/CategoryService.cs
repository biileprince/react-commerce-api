using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Features.Categories.DTOs;

namespace ReactCommerce.Api.Features.Categories;

public class CategoryService : ICategoryService
{
    private readonly AppDbContext _db;

    public CategoryService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<CategoryResponse>> GetAllActiveAsync()
    {
        return await _db.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new CategoryResponse(
                c.Id, c.Name, c.Slug, c.Description, c.Icon, c.IsActive,
                c.Products.Count(p => !p.IsDeleted)))
            .ToListAsync();
    }
}
