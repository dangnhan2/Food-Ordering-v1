using FoodOrdering.Application;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Infrastructure.Data;
using FoodOrdering.Infrastructure.Repositories;
using FoodOrdering.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly FoodOrderingDbContext _context;
        

        public UnitOfWork(FoodOrderingDbContext context)
        {
            _context = context;
            Order = new OrderRepo(_context);
            Menu = new MenuRepo(_context);
            User = new UserRepo(_context);
            Category = new CategoryRepo(_context);
            Voucher = new VoucherRepo(_context);    
            RefreshToken = new RefreshTokenRepo(_context);
            Cart = new CartRepo(_context);
        }

        public IOrderRepo Order { get; }

        public IMenuRepo Menu { get; }

        public IUserRepo User { get; }

        public ICategoryRepo Category { get; }

        public IVoucherRepo Voucher { get; }

        public IRefreshTokenRepo RefreshToken { get; }

        public ICartRepo Cart { get; }

        public void Dispose()
        {
            _context.Dispose();
        }

        public async Task SaveChangeAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
