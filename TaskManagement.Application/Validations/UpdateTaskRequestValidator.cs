using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaskManagement.Dtos;

namespace TaskManagement.Application.Validations
{
    public class UpdateTaskRequestValidator : AbstractValidator<UpdateTaskDto>
    {
        public UpdateTaskRequestValidator()
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
}
