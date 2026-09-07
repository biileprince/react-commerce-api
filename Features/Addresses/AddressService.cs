using Microsoft.EntityFrameworkCore;
using ReactCommerce.Api.Data;
using ReactCommerce.Api.Entities;
using ReactCommerce.Api.Features.Addresses.DTOs;

namespace ReactCommerce.Api.Features.Addresses;

public class AddressService : IAddressService
{
    private readonly AppDbContext _db;

    public AddressService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<AddressResponse>> GetAllAsync(Guid userId)
    {
        return await _db.Addresses
            .Where(a => a.UserId == userId)
            .Select(a => ToDto(a))
            .ToListAsync();
    }

    public async Task<AddressResponse> CreateAsync(Guid userId, AddressRequest req)
    {
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

        _db.Addresses.Add(address);
        await _db.SaveChangesAsync();

        return ToDto(address);
    }

    public async Task<AddressResponse?> UpdateAsync(Guid userId, Guid addressId, AddressRequest req)
    {
        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        if (address is null) return null;

        address.FullName = req.FullName;
        address.PhoneNumber = req.PhoneNumber;
        address.AddressLine1 = req.AddressLine1;
        address.AddressLine2 = req.AddressLine2;
        address.City = req.City;
        address.Region = req.Region;
        address.District = req.District;
        address.Landmark = req.Landmark;
        address.IsDefault = req.IsDefault;

        await _db.SaveChangesAsync();

        return ToDto(address);
    }

    public async Task<bool> DeleteAsync(Guid userId, Guid addressId)
    {
        var address = await _db.Addresses.FirstOrDefaultAsync(a => a.Id == addressId && a.UserId == userId);
        if (address is null) return false;

        _db.Addresses.Remove(address);
        await _db.SaveChangesAsync();

        return true;
    }

    private static AddressResponse ToDto(Address a) => new(
        a.Id, a.FullName, a.PhoneNumber, a.AddressLine1, a.AddressLine2,
        a.City, a.Region, a.District, a.Landmark, a.IsDefault);
}
