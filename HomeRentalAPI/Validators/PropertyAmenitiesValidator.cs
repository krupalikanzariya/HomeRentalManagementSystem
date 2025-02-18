using FluentValidation;
using HomeRentalAPI.Models;

namespace HomeRentalAPI.Validatiors
{
    public class PropertyAmenitiesValidator : AbstractValidator<PropertyAmenitiesModel>
    {
        public PropertyAmenitiesValidator()
        {
            RuleFor(x => x.PropertyID)
                .NotEmpty().WithMessage("PropertyID is required.")
                .GreaterThan(0).WithMessage("PropertyID must be a positive number.");
            RuleFor(x => x.AmenityID)
                .NotEmpty().WithMessage("AmenityID is required.")
                .GreaterThan(0).WithMessage("AmenityID must be a positive number.");
        }
    }
}
