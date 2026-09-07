namespace ReactCommerce.Api.Features.Addresses.DTOs;

public record AddressRequest(
    string FullName, string PhoneNumber, string AddressLine1,
    string? AddressLine2, string City, string Region,
    string? District, string? Landmark, bool IsDefault = false);

public record AddressResponse(
    Guid Id, string FullName, string PhoneNumber, string AddressLine1,
    string? AddressLine2, string City, string Region,
    string? District, string? Landmark, bool IsDefault);
