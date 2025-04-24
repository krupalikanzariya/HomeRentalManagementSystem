using FluentValidation;
using HomeRentalFrontEnd.Models;

namespace HomeRentalFrontEnd.Validatiors
{
    public class PropertiesValidator : AbstractValidator<PropertiesModel>
    {
        public PropertiesValidator()
        {
            RuleFor(x => x.HostID)
                .NotNull().WithMessage("HostID is required.")
                .GreaterThan(0).WithMessage("HostID must be a valid positive number.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Address)
                .NotEmpty().WithMessage("Address is required.")
                .MaximumLength(200).WithMessage("Address must not exceed 200 characters.");

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(50).WithMessage("City must not exceed 50 characters.");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(50).WithMessage("State must not exceed 50 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(50).WithMessage("Country must not exceed 50 characters.");

            RuleFor(x => x.PricePerNight)
                .GreaterThan(0).WithMessage("Price per night must be a positive number.");

            RuleFor(x => x.MaxGuests)
                .NotEmpty().WithMessage("Maximum guests is required.")
                .GreaterThan(0).WithMessage("Maximum guests must be a positive number.")
                .LessThanOrEqualTo(50).WithMessage("Maximum guests must not exceed 50.");

            RuleFor(x => x.Bedrooms)
                .NotEmpty().WithMessage("Number of bedrooms are required.")
                .GreaterThan(0).WithMessage("Number of bedrooms must be at least 1.")
                .LessThanOrEqualTo(20).WithMessage("Number of bedrooms must not exceed 20.");
        }
    }
}
