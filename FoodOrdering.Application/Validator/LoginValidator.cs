using FluentValidation;
using FoodOrdering.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class LoginValidator : AbstractValidator<LoginRequest>
    {
        public LoginValidator() {
            // Kiểm tra trường Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
                .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");

            // Kiểm tra trường Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");
        }
    }
}
