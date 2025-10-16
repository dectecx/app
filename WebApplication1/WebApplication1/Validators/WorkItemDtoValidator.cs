using FluentValidation;
using WebApplication1.DTOs;

namespace WebApplication1.Validators
{
    public class CreateWorkItemDtoValidator : AbstractValidator<CreateWorkItemDto>
    {
        public CreateWorkItemDtoValidator()
        {
            RuleFor(x => x.Title).NotEmpty().MaximumLength(256);
        }
    }

    public class UpdateWorkItemDtoValidator : AbstractValidator<UpdateWorkItemDto>
    {
        public UpdateWorkItemDtoValidator()
        {
            RuleFor(x => x.Title).MaximumLength(256);
            RuleFor(x => x.Status).MaximumLength(50);
        }
    }
}
