using FluentValidation;
using Food_Ordering.DTOs.Request;

namespace Food_Ordering.Validations
{
    public class OrderValidator : AbstractValidator<OrderRequest>
    {
        public OrderValidator() {
            // Kiểm tra UserId
            RuleFor(x => x.UserId)
                .NotEmpty().WithMessage("UserId không được để trống.");
            
            // Kiểm tra PaymentMethod
            RuleFor(x => x.PaymentMethod)
                .NotEmpty().WithMessage("Phương thức thanh toán không được để trống.")
                .MaximumLength(50).WithMessage("Phương thức thanh toán không được vượt quá 50 ký tự.");

            // Kiểm tra Items
            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Đơn hàng phải có ít nhất một món hàng.");

            // Áp dụng validator cho từng item trong danh sách
            RuleForEach(x => x.Items).SetValidator(new OrderItemValidator());
        }
    }
}
