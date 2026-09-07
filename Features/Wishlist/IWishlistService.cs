namespace ReactCommerce.Api.Features.Wishlist;

public interface IWishlistService
{
    Task<List<Guid>> GetWishlistAsync(Guid userId);
    Task<bool> AddToWishlistAsync(Guid userId, Guid productId);
    Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId);
}
