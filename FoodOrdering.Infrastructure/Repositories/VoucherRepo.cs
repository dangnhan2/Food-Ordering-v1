using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repository
{
    public class VoucherRepo : GenericRepo<Voucher>, IVoucherRepo
    {   
        private readonly FoodOrderingDbContext _context;
        public VoucherRepo(FoodOrderingDbContext context) : base(context) {
           _context = context;
        }

        public async Task<int> CountAsync(Guid id)
        {
            return await _context.Voucher.CountAsync(v => v.Id == id);
        }
    }
}
