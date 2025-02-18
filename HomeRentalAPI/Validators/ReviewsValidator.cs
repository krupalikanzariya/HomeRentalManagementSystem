using FluentValidation;
using HomeRentalAPI.Models;

namespace HomeRentalAPI.Validatiors
{
    public class ReviewsValidator : AbstractValidator<ReviewsModel>
    {
        public ReviewsValidator()
        {
            RuleFor(x => x.UserID)
                .GreaterThan(0).WithMessage("UserID must be a valid positive number.");

            RuleFor(x => x.PropertyID)
                .GreaterThan(0).WithMessage("PropertyID must be a valid positive number.");

            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("Rating is required.")
                .InclusiveBetween(1,5).WithMessage("Rating must be between 1 to 5.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required.");
        }
    }
}
