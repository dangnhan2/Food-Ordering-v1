using FluentValidation;
using FoodOrdering.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class UserValidation : AbstractValidator<UserRequest>
    {
        public UserValidation() {
            RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ và tên không được để trống.")
            .Length(5, 100).WithMessage("Họ và tên phải dài từ 5 đến 100 ký tự.");

            // Kiểm tra trường Email
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Địa chỉ email không được để trống.")
                .EmailAddress().WithMessage("Địa chỉ email không hợp lệ. Vui lòng kiểm tra lại định dạng.");
        }
    }
}
