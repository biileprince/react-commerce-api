using FluentValidation;
using ReactCommerce.Api.Features.Addresses.DTOs;

namespace ReactCommerce.Api.Features.Addresses.Validators;

public class AddressRequestValidator : AbstractValidator<AddressRequest>
{
    public AddressRequestValidator()
    {
        RuleFor(x => x.FullName).NotEmpty().WithMessage("Full Name is required");
        RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Phone Number is required");
        RuleFor(x => x.AddressLine1).NotEmpty().WithMessage("Address Line 1 is required");
        RuleFor(x => x.City).NotEmpty().WithMessage("City is required");
        RuleFor(x => x.Region).NotEmpty().WithMessage("Region is required");
    }
}
