using ReactCommerce.Api.Features.Addresses.DTOs;

namespace ReactCommerce.Api.Features.Addresses;

public interface IAddressService
{
    Task<List<AddressResponse>> GetAllAsync(Guid userId);
    Task<AddressResponse> CreateAsync(Guid userId, AddressRequest request);
    Task<AddressResponse?> UpdateAsync(Guid userId, Guid addressId, AddressRequest request);
    Task<bool> DeleteAsync(Guid userId, Guid addressId);
}
