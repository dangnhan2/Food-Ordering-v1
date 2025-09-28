﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class Voucher
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string DiscountType { get; set; } // "percent" | "fixed"
        public decimal DiscountValue { get; set; }
        public decimal? MaxDiscount { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int? UsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int? PerUserLimit { get; set; }
        public bool IsActive { get; set; }
        public ICollection<VoucherRedemptions> VoucherRedemptions { get; set; } = new List<VoucherRedemptions>();
    }
}
