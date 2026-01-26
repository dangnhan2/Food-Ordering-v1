using FluentValidation;
using FoodOrdering.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class UserValidator : AbstractValidator<UserRequestDto>
    {
        public UserValidator() {           
            // Kiểm tra trường Email
            RuleFor(x => x.PhoneNumber)
                .NotEmpty().WithMessage("Số điện thoại không được để trống.")
                .Matches(@"^0\d{9,10}$").WithMessage("Số điện thoại không hợp lệ. Vui lòng nhập đúng định dạng (ví dụ: 0987654321).");
        }
    }
}
