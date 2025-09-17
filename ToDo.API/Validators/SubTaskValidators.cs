using FluentValidation;
using ToDo.API.Dtos.SubTaskDtos;

namespace ToDo.API.Validators
{
    public class CreateSubTaskValidator : AbstractValidator<CreateSubTaskRequestDto>
    {
        public CreateSubTaskValidator()
        {
            RuleFor(x => x.ToDoId)
                .GreaterThan(0).WithMessage("Valid ToDo ID is required");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("SubTask title is required")
                .Length(1, 200).WithMessage("Title must be between 1 and 200 characters");
        }
    }

    public class UpdateSubTaskValidator : AbstractValidator<UpdateSubTaskRequestDto>
    {
        public UpdateSubTaskValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid SubTask ID");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("SubTask title is required")
                .Length(1, 200).WithMessage("Title must be between 1 and 200 characters");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value");
        }
    }
}
