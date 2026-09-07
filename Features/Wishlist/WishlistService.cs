using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;

namespace ReactCommerce.Api.Features.Wishlist;

public class WishlistService : IWishlistService
{
    private readonly AppDbContext _db;

    public WishlistService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<Guid>> GetWishlistAsync(Guid userId)
    {
        return await _db.WishlistItems
            .Where(w => w.UserId == userId)
            .Select(w => w.ProductId)
            .ToListAsync();
    }

    public async Task<bool> AddToWishlistAsync(Guid userId, Guid productId)
    {
        var exists = await _db.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        if (exists) return false;

        _db.WishlistItems.Add(new Entities.WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = productId,
        });
        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId)
    {
        var item = await _db.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item is null) return false;

        _db.WishlistItems.Remove(item);
        await _db.SaveChangesAsync();

        return true;
    }
}
