using System.Security.Claims;
using ReactCommerce.Api.Features.Orders.DTOs;
using ReactCommerce.Api.Shared;

namespace ReactCommerce.Api.Features.Orders;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/orders").WithTags("Orders");

        group.MapPost("/", CreateOrder)
             .RequireAuthorization()
             .AddEndpointFilter<ValidationFilter<CreateOrderRequest>>();
             
        group.MapGet("/", GetOrders).RequireAuthorization();
        group.MapGet("/{id:guid}", GetOrder).RequireAuthorization();
    }

    private static async Task<IResult> CreateOrder(CreateOrderRequest req, ClaimsPrincipal claims, IOrderService orderService)
    {
        var userId = claims.RequireUserId();
        var (response, error) = await orderService.CreateOrderAsync(userId, req);

        if (error is not null)
            return Results.BadRequest(ApiResponse<object>.Fail(error));

        return Results.Created($"/api/orders/{response!.Id}", ApiResponse<OrderResponse>.Ok(response));
    }

    private static async Task<IResult> GetOrders(ClaimsPrincipal claims, IOrderService orderService, int page = 1, int limit = 10)
    {
        var userId = claims.RequireUserId();
        var response = await orderService.GetOrdersAsync(userId, page, limit);
        return Results.Ok(response);
    }

    private static async Task<IResult> GetOrder(Guid id, ClaimsPrincipal claims, IOrderService orderService)
    {
        var userId = claims.RequireUserId();
        var response = await orderService.GetOrderAsync(userId, id);
        
        if (response is null) return Results.NotFound();

        return Results.Ok(ApiResponse<OrderResponse>.Ok(response));
    }
}
