using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public DateTimeOffset StartDate { get; set; }
        public DateTimeOffset EndDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int? PerUserLimit { get; set; }
        public bool IsActive { get; set; }

        public VoucherDTO() { }
        public VoucherDTO(Voucher voucher)
        {
            Id = voucher.Id;
            Code = voucher.Code; 
            Description = voucher.Description;
            DiscountType = voucher.DiscountType;
            DiscountValue = voucher.DiscountValue;
            MaxDiscount = voucher.MaxDiscount;
            MinOrderAmount = voucher.MinOrderAmount;
            StartDate = voucher.StartDate;
            EndDate = voucher.EndDate;
            UsageLimit = voucher.UsageLimit;
            UsedCount = voucher.UsedCount;
            PerUserLimit = voucher.PerUserLimit;
            IsActive = voucher.IsActive;  
        }
    }
}
