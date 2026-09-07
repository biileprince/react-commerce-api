using ReactCommerce.Api.Features.Orders.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Orders;

public interface IOrderService
{
    Task<(OrderResponse? Response, string? Error)> CreateOrderAsync(Guid userId, CreateOrderRequest request);
    Task<PaginatedResponse<OrderResponse>> GetOrdersAsync(Guid userId, int page = 1, int limit = 10);
    Task<OrderResponse?> GetOrderAsync(Guid userId, Guid orderId);
}
