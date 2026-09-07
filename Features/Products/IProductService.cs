using ReactCommerce.Api.Features.Products.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Products;

public interface IProductService
{
    Task<PaginatedResponse<ProductResponse>> GetProductsAsync(ProductQueryParams query);
    Task<List<ProductResponse>> GetFeaturedAsync(int limit = 6);
    Task<ProductResponse?> GetBySlugAsync(string slug);
    Task<List<ProductResponse>> GetRelatedAsync(Guid id, int limit = 4);
}
