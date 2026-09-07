using ReactCommerce.Api.Features.Categories.DTOs;

namespace ReactCommerce.Api.Features.Categories;

public interface ICategoryService
{
    Task<List<CategoryResponse>> GetAllActiveAsync();
}
