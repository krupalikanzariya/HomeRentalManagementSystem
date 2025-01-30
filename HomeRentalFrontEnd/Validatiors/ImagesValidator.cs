using FluentValidation;
using HomeRentalFrontEnd.Models;

namespace HomeRentalFrontEnd.Validatiors
{
    public class ImagesValidator : AbstractValidator<ImagesModel>
    {
        public ImagesValidator()
        {
            RuleFor(x => x.PropertyID)
                .GreaterThan(0).WithMessage("PropertyID must be a valid positive number.");
            RuleFor(x => x.ImageURL)
                .NotEmpty().WithMessage("Image URL is required.");

        }
    }
}
