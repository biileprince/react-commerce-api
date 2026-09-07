using System.Security.Claims;
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

    private static async Task<IResult> GetWishlist(ClaimsPrincipal claims, IWishlistService wishlistService)
    {
        var userId = claims.RequireUserId();
        var productIds = await wishlistService.GetWishlistAsync(userId);
        return Results.Ok(ApiResponse<List<Guid>>.Ok(productIds));
    }

    private static async Task<IResult> AddToWishlist(Guid productId, ClaimsPrincipal claims, IWishlistService wishlistService)
    {
        var userId = claims.RequireUserId();
        var added = await wishlistService.AddToWishlistAsync(userId, productId);
        
        if (!added) return Results.Ok(ApiResponse<object>.Ok(new { }, "Already in wishlist"));

        return Results.Created($"/api/wishlist", ApiResponse<object>.Ok(new { }, "Added to wishlist"));
    }

    private static async Task<IResult> RemoveFromWishlist(Guid productId, ClaimsPrincipal claims, IWishlistService wishlistService)
    {
        var userId = claims.RequireUserId();
        var removed = await wishlistService.RemoveFromWishlistAsync(userId, productId);

        if (!removed) return Results.NotFound();

        return Results.NoContent();
    }
}
