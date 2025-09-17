using FluentValidation;
using ToDo.API.Dtos.NotificationDtos;

namespace ToDo.API.Validators
{
    public class CreateNotificationValidator : AbstractValidator<CreateNotificationRequestDto>
    {
        public CreateNotificationValidator()
        {
            RuleFor(x => x.UserId)
                .GreaterThan(0).WithMessage("Valid User ID is required");

            RuleFor(x => x.ToDoId)
                .GreaterThan(0).WithMessage("Valid ToDo ID is required");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .Length(1, 500).WithMessage("Message must be between 1 and 500 characters");
        }
    }

    public class UpdateNotificationValidator : AbstractValidator<UpdateNotificationRequestDto>
    {
        public UpdateNotificationValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Notification ID");

            RuleFor(x => x.Message)
                .NotEmpty().WithMessage("Message is required")
                .Length(1, 500).WithMessage("Message must be between 1 and 500 characters");
        }
    }

    public class MarkAsReadValidator : AbstractValidator<MarkAsReadRequestDto>
    {
        public MarkAsReadValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0).WithMessage("Invalid Notification ID");
        }
    }
}
