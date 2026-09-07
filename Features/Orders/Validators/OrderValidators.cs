using FluentValidation;
using ReactCommerce.Api.Features.Orders.DTOs;

namespace ReactCommerce.Api.Features.Orders.Validators;

public class CreateOrderRequestValidator : AbstractValidator<CreateOrderRequest>
{
    public CreateOrderRequestValidator()
    {
        RuleFor(x => x.AddressId).NotEmpty().WithMessage("Shipping address is required");
        
        RuleFor(x => x.Items)
            .NotEmpty().WithMessage("Order must contain at least one item");
            
        RuleForEach(x => x.Items).ChildRules(items => 
        {
            items.RuleFor(i => i.ProductId).NotEmpty().WithMessage("Product ID is required");
            items.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        });
    }
}
