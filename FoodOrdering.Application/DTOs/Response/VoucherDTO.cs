using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Domain.Models;

namespace FoodOrdering.Application.DTOs.Response
{
    public class VoucherDTO
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; } // "percent" | "fixed"
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int? PerUserLimit { get; set; }
        public bool IsActive { get; set; }      
    }
}
