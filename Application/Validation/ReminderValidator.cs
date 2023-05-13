using Domain.Dtos;
using Domain.Entities;
using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Validation
{
    public class ReminderValidator : AbstractValidator<CreateReminderDto>
    { 
        public ReminderValidator()
        {
            RuleFor(x => x.Content).Length(0, 300);
            RuleFor(x => x.To).NotNull().EmailAddress();
            RuleFor(x => x.SendAt).NotNull();
        }
    }
}
