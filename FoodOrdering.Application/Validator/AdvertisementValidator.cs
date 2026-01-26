using FluentValidation;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class AdvertisementValidator : AbstractValidator<AdvertisementRequestDto>
    {
        public AdvertisementValidator() {
            // 1. Kiểm tra Tiêu đề
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Tiêu đề quảng cáo không được để trống.")
                .MaximumLength(200).WithMessage("Tiêu đề không được vượt quá 200 ký tự.");

            // 3. Kiểm tra AdTargetType và TargetKey
            // Giả sử nếu TargetType là Product hoặc Category thì TargetKey (ID) không được để trống
            RuleFor(x => x.TargetKey)
                .NotEmpty()
                .WithMessage("Từ khóa mục tiêu (TargetKey) không được để trống khi đã chọn loại mục tiêu.");

            // 4. Kiểm tra Thời gian StartAt và EndAt
            RuleFor(x => x.StartAt)
                .NotEmpty().WithMessage("Thời gian bắt đầu không được để trống.");

            RuleFor(x => x.EndAt)
                .GreaterThan(x => x.StartAt).WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu.");
        }
    }
}
