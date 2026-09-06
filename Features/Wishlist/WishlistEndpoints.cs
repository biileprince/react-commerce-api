using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Features.Products;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Wishlist;

public static class WishlistEndpoints
{
    public static void MapWishlistEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/wishlist")
            .WithTags("Wishlist")
            .RequireAuthorization();

        group.MapGet("/", GetWishlist);
        group.MapPost("/{productId:guid}", AddToWishlist);
        group.MapDelete("/{productId:guid}", RemoveFromWishlist);
    }

    private static async Task<IResult> GetWishlist(ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();
        var productIds = await db.WishlistItems
            .Where(w => w.UserId == userId)
            .Select(w => w.ProductId)
            .ToListAsync();

        return Results.Ok(ApiResponse<List<Guid>>.Ok(productIds));
    }

    private static async Task<IResult> AddToWishlist(Guid productId, ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();

        var exists = await db.WishlistItems
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        if (exists) return Results.Ok(ApiResponse<object>.Ok(new { }, "Already in wishlist"));

        db.WishlistItems.Add(new Entities.WishlistItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = productId,
        });
        await db.SaveChangesAsync();

        return Results.Created($"/api/wishlist", ApiResponse<object>.Ok(new { }, "Added to wishlist"));
    }

    private static async Task<IResult> RemoveFromWishlist(Guid productId, ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();
        var item = await db.WishlistItems
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item is null) return Results.NotFound();

        db.WishlistItems.Remove(item);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }
}
