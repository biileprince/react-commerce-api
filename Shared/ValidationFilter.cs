using FluentValidation;

namespace ReactCommerce.Api.Shared;

public class ValidationFilter<T> : IEndpointFilter
{
    public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
    {
        var validator = context.HttpContext.RequestServices.GetService<IValidator<T>>();
        
        if (validator is not null)
        {
            var entity = context.Arguments
                .OfType<T>()
                .FirstOrDefault(a => a?.GetType() == typeof(T));

            if (entity is not null)
            {
                var validationResult = await validator.ValidateAsync(entity);
                if (!validationResult.IsValid)
                {
                    return Results.ValidationProblem(validationResult.ToDictionary());
                }
            }
        }

        return await next(context);
    }
}
