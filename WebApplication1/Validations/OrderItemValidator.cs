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

        }
    }
}
