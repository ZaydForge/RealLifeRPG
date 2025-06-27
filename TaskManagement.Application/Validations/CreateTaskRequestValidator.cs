using FluentValidation;
using TaskManagement.Dtos;

namespace TaskManagement.Rules;

public class CreateTaskRequestValidator : AbstractValidator<CreateTaskDto>
{
    public CreateTaskRequestValidator()
    {
        RuleFor(r => r.Title)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(r => r.Description)
            .MaximumLength(255);

        RuleFor(r => r.Category)
            .NotEmpty()
            .IsInEnum();

        RuleFor(r => r.EXPValue)
            .GreaterThan(0);

        
    }
}
