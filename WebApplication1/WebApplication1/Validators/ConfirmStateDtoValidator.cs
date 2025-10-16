using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators
{
    public class ConfirmStateDtoValidator : AbstractValidator<ConfirmStateDto>
    {
        public ConfirmStateDtoValidator()
        {
            RuleFor(x => x.States).NotNull().NotEmpty();
        }
    }
}
