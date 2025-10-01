using FluentValidation;
using FoodOrdering.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator() {
            // Kiểm tra trường Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
                .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");

            // Kiểm tra trường FullName
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ và tên không được để trống.")
                .MaximumLength(100).WithMessage("Họ và tên không được vượt quá 100 ký tự.");

            // Kiểm tra trường Password
            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");
                
            // Kiểm tra trường ConfirmPassword (Xác nhận Mật khẩu)
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.")
                .Equal(x => x.Password).WithMessage("Xác nhận mật khẩu không khớp với Mật khẩu.");
        }
    }
}
