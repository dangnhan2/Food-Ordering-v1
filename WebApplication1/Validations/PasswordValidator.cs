using FluentValidation;
using Food_Ordering.DTOs.Request;

namespace Food_Ordering.Validations
{
    public class PasswordValidator : AbstractValidator<PasswordRequest>
    {
        public PasswordValidator()
        {
            RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Mật khẩu cũ không được để trống.");

            // Rule for NewPassword
            RuleFor(x => x.NewPassword)
                .NotEmpty().WithMessage("Mật khẩu mới không được để trống.")
                .MinimumLength(6).WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự.");
                
            // Rule for ConfirmPassword
            RuleFor(x => x.ConfirmPassword)
                .NotEmpty().WithMessage("Xác nhận mật khẩu không được để trống.")
                .Equal(x => x.NewPassword).WithMessage("Xác nhận mật khẩu không khớp.");
        }
    }
}
