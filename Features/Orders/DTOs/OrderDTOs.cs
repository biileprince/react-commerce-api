using ReactCommerce.Api.Features.Addresses.DTOs;

namespace ReactCommerce.Api.Features.Orders.DTOs;

public record CreateOrderRequest(Guid AddressId, List<OrderItemRequest> Items);
public record OrderItemRequest(Guid ProductId, int Quantity);

public record OrderResponse(
    Guid Id, decimal Subtotal, decimal ShippingFee, decimal Total,
    string Currency, string Status, DateTime CreatedAt,
    AddressResponse ShippingAddress, List<OrderItemResponse> Items);

public record OrderItemResponse(
    Guid ProductId, string ProductName, decimal UnitPrice,
    int Quantity, string? ImageUrl);
