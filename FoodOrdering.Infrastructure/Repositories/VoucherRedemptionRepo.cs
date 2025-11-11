using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using FoodOrdering.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repositories
{
    public class VoucherRedemptionRepo : GenericRepo<VoucherRedemptions>, IVoucherRedemptionRepo
    {   
        private readonly FoodOrderingDbContext _context;
        public VoucherRedemptionRepo(FoodOrderingDbContext context) : base(context) {
           _context = context;
        }

        public async Task<int> TodayCountAsync(Guid userId, Guid voucherId)
        {
            return await _context.VoucherRedemptions.CountAsync(
                v => v.UserID == userId 
                && v.VoucherID == voucherId 
                && v.RedeemedAt.Date == DateTime.UtcNow.Date);
        }
    }
}
