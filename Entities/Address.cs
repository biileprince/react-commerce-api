namespace ReactCommerce.Api.Entities;

public class Address
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public required string FullName { get; set; }
    public required string PhoneNumber { get; set; }
    public required string AddressLine1 { get; set; }
    public string? AddressLine2 { get; set; }
    public required string City { get; set; }
    public required string Region { get; set; }
    public string? District { get; set; }
    public string? Landmark { get; set; }
    public bool IsDefault { get; set; }

    public User User { get; set; } = null!;
}
