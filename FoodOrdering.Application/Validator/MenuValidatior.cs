using FluentValidation;
using FoodOrdering.Application.DTOs.Request;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Validator
{
    public class MenuValidatior : AbstractValidator<MenuRequest>
    {
        public MenuValidatior() {
            // Kiểm tra trường Name
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Tên món ăn không được để trống.")
                .Length(3, 100).WithMessage("Tên món ăn phải có độ dài từ 3 đến 100 ký tự.");

            // Kiểm tra trường CategoriesId
            RuleFor(x => x.CategoriesId)
                .NotEmpty().WithMessage("ID danh mục không được để trống.");

            // Kiểm tra trường Description
            // Vì là nullable, chỉ cần kiểm tra độ dài tối đa
            RuleFor(x => x.Description)
                .MaximumLength(500).WithMessage("Mô tả không được vượt quá 500 ký tự.");

            // Kiểm tra trường Price
            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Giá bán không được là số âm.")
                .LessThanOrEqualTo(10000000).WithMessage("Giá bán tối đa là 10.000.000 VNĐ."); // Giả định mức giá tối đa

            // Kiểm tra trường StockQuantity
            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("Số lượng tồn kho không được là số âm.");
        }
    }
}
