using FluentValidation;
using Food_Ordering.DTOs.Request;

namespace Food_Ordering.Validations
{
    public class MenuCategoryValidator : AbstractValidator<MenuCategoryRequest>
    {
        public MenuCategoryValidator() {
            RuleFor(x => x.Name)
             .NotEmpty().WithMessage("Tên danh mục không được để trống.")
             .Length(3, 100).WithMessage("Tên danh mục phải có độ dài từ 3 đến 100 ký tự.");

            // Quy tắc kiểm tra cho trường Description
            // Vì Description là nullable string (string?), bạn có thể thêm các quy tắc kiểm tra nếu cần
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");
        }
    }
}
