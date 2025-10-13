using FluentValidation;
using FoodOrdering.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class VoucherValidator : AbstractValidator<VoucherRequest>
    {
        public VoucherValidator() {
            // 1. Kiểm tra trường Code
            RuleFor(x => x.Code)
                .NotEmpty().WithMessage("Mã giảm giá không được để trống.")
                .Length(4, 20).WithMessage("Mã giảm giá phải dài từ 4 đến 20 ký tự.");

            // 3. Kiểm tra trường DiscountType
            RuleFor(x => x.DiscountType)
                .NotEmpty().WithMessage("Loại giảm giá không được để trống.")
                .Must(type => type.ToLower() == "percent" || type.ToLower() == "fixed")
                .WithMessage("Loại giảm giá không hợp lệ. Chỉ chấp nhận 'percent' hoặc 'fixed'.");

            // 4. Kiểm tra trường DiscountValue
            RuleFor(x => x.DiscountValue)
                .GreaterThan(0).WithMessage("Giá trị giảm giá phải lớn hơn 0.");

            // 5. Logic cho DiscountType = "percent"
            When(x => x.DiscountType.ToLower() == "percent", () =>
            {
                RuleFor(x => x.DiscountValue)
                    .LessThanOrEqualTo(100).WithMessage("Nếu là giảm theo phần trăm, giá trị phải nhỏ hơn hoặc bằng 100.");

                RuleFor(x => x.MaxDiscount)
                    .NotNull().WithMessage("Nếu là giảm theo phần trăm, cần chỉ rõ giới hạn giảm tối đa (MaxDiscount).")
                    .GreaterThan(0).WithMessage("Giới hạn giảm tối đa phải lớn hơn 0.");
            });

            // 6. Kiểm tra trường MaxDiscount
            RuleFor(x => x.MaxDiscount)
                .GreaterThanOrEqualTo(0).When(x => x.MaxDiscount.HasValue)
                .WithMessage("Giới hạn giảm tối đa không được là số âm.");

            // 7. Kiểm tra trường MinOrderAmount
            RuleFor(x => x.MinOrderAmount)
                .GreaterThanOrEqualTo(0).When(x => x.MinOrderAmount.HasValue)
                .WithMessage("Giá trị đơn hàng tối thiểu không được là số âm.");

            // 8. Kiểm tra trường StartDate và EndDate
            RuleFor(x => x.StartDate)
                .NotEmpty().WithMessage("Ngày bắt đầu không được để trống.");

            RuleFor(x => x.EndDate)
                .NotEmpty().WithMessage("Ngày kết thúc không được để trống.")
                .GreaterThan(x => x.StartDate).WithMessage("Ngày kết thúc phải sau Ngày bắt đầu.");

            // 9. Kiểm tra trường UsageLimit và PerUserLimit
            RuleFor(x => x.UsageLimit)
                .GreaterThanOrEqualTo(0).When(x => x.UsageLimit.HasValue)
                .WithMessage("Giới hạn sử dụng toàn bộ không được là số âm.");

            RuleFor(x => x.PerUserLimit)
                .GreaterThanOrEqualTo(0).When(x => x.PerUserLimit.HasValue)
                .WithMessage("Giới hạn sử dụng cho mỗi người dùng không được là số âm.");
        }
    }
}
