using FluentValidation;
using HomeRentalAPI.Models;

namespace HomeRentalAPI.Validatiors
{
    public class AmenitiesValidator : AbstractValidator<AmenitiesModel>
    {
        public AmenitiesValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Amenity name is required.")
                .MaximumLength(50).WithMessage("Amenity name must not exceed 50 characters.");

        }
    }
}
