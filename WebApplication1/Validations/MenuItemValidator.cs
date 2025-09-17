using FluentValidation;
using Food_Ordering.DTOs.Request;

namespace Food_Ordering.Validations
{
    public class MenuItemValidator : AbstractValidator<MenuItemRequest>
    {
        public MenuItemValidator() {
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Tên món ăn không được để trống.")
             .Length(3, 150).WithMessage("Tên món ăn phải có độ dài từ 3 đến 150 ký tự.");

            // Kiểm tra trường MenuCategoriesId
            RuleFor(x => x.MenuCategoriesId)
                .NotEmpty().WithMessage("Danh mục không được để trống.");

            // Kiểm tra trường Description
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");

            // Kiểm tra trường Price
            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Giá phải lớn hơn 0.");

            RuleFor(x => x.IsAvailable)
                .NotNull().WithMessage("Trạng thái không được để trống");

            // Kiểm tra trường StockQuantity
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho không được nhỏ hơn 0.");
        }
    }
}
