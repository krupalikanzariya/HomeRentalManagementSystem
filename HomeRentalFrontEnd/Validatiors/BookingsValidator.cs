using FluentValidation;
using HomeRentalFrontEnd.Models;

namespace HomeRentalFrontEnd.Validatiors
{
    public class BookingsValidator : AbstractValidator<BookingsModel>
    {
        public BookingsValidator()
        {
            RuleFor(x => x.UserID)
                .GreaterThan(0).WithMessage("UserID must be a valid positive number.");

            RuleFor(x => x.PropertyID)
                .GreaterThan(0).WithMessage("PropertyID must be a valid positive number.");

            RuleFor(x => x.CheckInDate)
                .NotEmpty().WithMessage("Check-in date is required.")
                .GreaterThan(DateTime.Now).WithMessage("Check-in date must be in the future.");

            RuleFor(x => x.CheckOutDate)
                .NotEmpty().WithMessage("Check-out date is required.")
                .GreaterThan(x => x.CheckInDate).WithMessage("Check-out date must be after check-in date.");

            RuleFor(x => x.Guests)
                .GreaterThan(0).WithMessage("Guests must be a positive number.");

            RuleFor(x => x.TotalPrice)
                .GreaterThan(0).WithMessage("Total price must be a positive number.");
        }
    }
}
