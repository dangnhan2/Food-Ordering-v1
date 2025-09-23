using FluentValidation;
using Food_Ordering.DTOs.Request;

namespace Food_Ordering.Validations
{
    public class OrderItemValidator : AbstractValidator<OrderItemRequest>
    {
        public OrderItemValidator() {
            // Kiểm tra trường MenuItemsId
            RuleFor(x => x.MenuItemsId)
                .NotEmpty().WithMessage("Id món ăn không được để trống.");

            // Kiểm tra trường Quantity
            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Số lượng phải lớn hơn 0.");

            // Kiểm tra trường UnitPrice
            RuleFor(x => x.UnitPrice)
                .GreaterThan(0).WithMessage("Giá phải lớn hơn 0.");

            // Kiểm tra trường SubTotal
            RuleFor(x => x.SubTotal)
                .GreaterThan(0).WithMessage("Tổng phụ phải lớn hơn 0.");

            // Kiểm tra xem SubTotal có khớp với Quantity * UnitPrice hay không
            RuleFor(x => x.SubTotal)
                .Must((request, subTotal) => subTotal == request.Quantity * request.UnitPrice)
                .WithMessage("Tổng phụ không khớp với tích của số lượng và đơn giá.");
        }
    }
}
