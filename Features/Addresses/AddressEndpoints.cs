using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Entities;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Addresses;

public record AddressRequest(
    string FullName, string PhoneNumber, string AddressLine1,
    string? AddressLine2, string City, string Region,
    string? District, string? Landmark, bool IsDefault = false);

public record AddressResponse(
    Guid Id, string FullName, string PhoneNumber, string AddressLine1,
    string? AddressLine2, string City, string Region,
    string? District, string? Landmark, bool IsDefault);

public static class AddressEndpoints
{
    public static void MapAddressEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/addresses")
            .WithTags("Addresses")
            .RequireAuthorization();

        group.MapGet("/", GetAll);
        group.MapPost("/", Create);
        group.MapPut("/{id:guid}", Update);
        group.MapDelete("/{id:guid}", Delete);
    }

    private static async Task<IResult> GetAll(ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();
        var addresses = await db.Addresses
            .Where(a => a.UserId == userId)
            .Select(a => ToDto(a))
            .ToListAsync();

        return Results.Ok(ApiResponse<List<AddressResponse>>.Ok(addresses));
    }

    private static async Task<IResult> Create(AddressRequest req, ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();

        var address = new Address
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FullName = req.FullName,
            PhoneNumber = req.PhoneNumber,
            AddressLine1 = req.AddressLine1,
            AddressLine2 = req.AddressLine2,
            City = req.City,
            Region = req.Region,
            District = req.District,
            Landmark = req.Landmark,
            IsDefault = req.IsDefault,
        };

        db.Addresses.Add(address);
        await db.SaveChangesAsync();

        return Results.Created($"/api/addresses/{address.Id}", ApiResponse<AddressResponse>.Ok(ToDto(address)));
    }

    private static async Task<IResult> Update(Guid id, AddressRequest req, ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();
        var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (address is null) return Results.NotFound();

        address.FullName = req.FullName;
        address.PhoneNumber = req.PhoneNumber;
        address.AddressLine1 = req.AddressLine1;
        address.AddressLine2 = req.AddressLine2;
        address.City = req.City;
        address.Region = req.Region;
        address.District = req.District;
        address.Landmark = req.Landmark;
        address.IsDefault = req.IsDefault;

        await db.SaveChangesAsync();

        return Results.Ok(ApiResponse<AddressResponse>.Ok(ToDto(address)));
    }

    private static async Task<IResult> Delete(Guid id, ClaimsPrincipal claims, AppDbContext db)
    {
        var userId = claims.RequireUserId();
        var address = await db.Addresses.FirstOrDefaultAsync(a => a.Id == id && a.UserId == userId);
        if (address is null) return Results.NotFound();

        db.Addresses.Remove(address);
        await db.SaveChangesAsync();

        return Results.NoContent();
    }

    private static AddressResponse ToDto(Address a) => new(
        a.Id, a.FullName, a.PhoneNumber, a.AddressLine1, a.AddressLine2,
        a.City, a.Region, a.District, a.Landmark, a.IsDefault);
}
