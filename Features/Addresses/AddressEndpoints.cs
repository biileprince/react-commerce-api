using System.Security.Claims;
using ReactCommerce.Api.Features.Addresses.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Addresses;

public static class AddressEndpoints
{
    public static void MapAddressEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/addresses")
            .WithTags("Addresses")
            .RequireAuthorization();

        group.MapGet("/", GetAll);
        
        group.MapPost("/", Create)
             .AddEndpointFilter<ValidationFilter<AddressRequest>>();
             
        group.MapPut("/{id:guid}", Update)
             .AddEndpointFilter<ValidationFilter<AddressRequest>>();
             
        group.MapDelete("/{id:guid}", Delete);
    }

    private static async Task<IResult> GetAll(ClaimsPrincipal claims, IAddressService addressService)
    {
        var userId = claims.RequireUserId();
        var addresses = await addressService.GetAllAsync(userId);
        return Results.Ok(ApiResponse<List<AddressResponse>>.Ok(addresses));
    }

    private static async Task<IResult> Create(AddressRequest req, ClaimsPrincipal claims, IAddressService addressService)
    {
        var userId = claims.RequireUserId();
        var result = await addressService.CreateAsync(userId, req);
        return Results.Created($"/api/addresses/{result.Id}", ApiResponse<AddressResponse>.Ok(result));
    }

    private static async Task<IResult> Update(Guid id, AddressRequest req, ClaimsPrincipal claims, IAddressService addressService)
    {
        var userId = claims.RequireUserId();
        var result = await addressService.UpdateAsync(userId, id, req);
        if (result is null) return Results.NotFound();

        return Results.Ok(ApiResponse<AddressResponse>.Ok(result));
    }

    private static async Task<IResult> Delete(Guid id, ClaimsPrincipal claims, IAddressService addressService)
    {
        var userId = claims.RequireUserId();
        var deleted = await addressService.DeleteAsync(userId, id);
        if (!deleted) return Results.NotFound();

        return Results.NoContent();
    }
}
