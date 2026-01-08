using FluentValidation;
using FoodOrdering.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class ResendEmailValidator : AbstractValidator<ResendEmailRequestDto>
    {
        public ResendEmailValidator()
        {
            RuleFor(x => x.UserEmail)
               .NotEmpty().WithMessage("Email không được để trống.")
               .EmailAddress().WithMessage("Định dạng email không hợp lệ.");
        }
    }
}
