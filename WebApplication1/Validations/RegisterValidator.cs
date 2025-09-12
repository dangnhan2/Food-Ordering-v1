using FluentValidation;
using Food_Ordering.DTOs.Request;

namespace Food_Ordering.Validations
{
    public class RegisterValidator : AbstractValidator<RegisterRequest>
    {
        public RegisterValidator()
        {
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ và tên không được để trống.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
                .EmailAddress().WithMessage("Địa chỉ email không hợp lệ.");

            RuleFor(x => x.Password)
                .NotEmpty().WithMessage("Mật khẩu không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu phải có ít nhất 6 ký tự.");

            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.")
                .Equal(x => x.Password).WithMessage("Xác nhận mật khẩu không khớp với mật khẩu đã nhập.");
        }
    }
}
