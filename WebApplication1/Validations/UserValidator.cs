using FluentValidation;
using Food_Ordering.DTOs.Request;

namespace Food_Ordering.Validations
{
    public class UserValidator : AbstractValidator<UserRequest>
    {
        public UserValidator() {
            RuleFor(x => x.UserName)
              .NotEmpty().WithMessage("Tên đăng nhập không được để trống.");
            
            // Kiểm tra trường FullName
            RuleFor(x => x.FullName)
                .NotEmpty().WithMessage("Họ và tên không được để trống.");

            // Kiểm tra trường Phone
            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.")
                .Matches(@"^0\d{9,10}$").WithMessage("Số điện thoại không hợp lệ. Vui lòng nhập đúng định dạng (ví dụ: 0987654321).");

        }
    }
}
