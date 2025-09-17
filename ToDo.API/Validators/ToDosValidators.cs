using FluentValidation;
using ToDo.API.Dtos.ToDosDtos;
using ToDo.Data.Entities.@enum;

namespace ToDo.API.Validators
{
    public class CreateToDosValidator : AbstractValidator<CreateToDosRequestDto>
    {
        public CreateToDosValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .Length(1, 200).WithMessage("Title must be between 1 and 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.ToDoAt)
                .Must(date => !date.HasValue || date >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("ToDoAt date cannot be in the past");
        }
    }

    public class UpdateToDosValidator : AbstractValidator<UpdateToDosRequestDto>
    {
        public UpdateToDosValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid ToDo ID");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required")
                .Length(1, 200).WithMessage("Title must be between 1 and 200 characters");

            RuleFor(x => x.Description)
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters");

            RuleFor(x => x.Status)
                .IsInEnum().WithMessage("Invalid status value");

            RuleFor(x => x.ToDoAt)
                .Must(date => !date.HasValue || date >= DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("ToDoAt date cannot be in the past");
        }
    }
}
