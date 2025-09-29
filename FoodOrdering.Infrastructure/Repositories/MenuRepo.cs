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
    public class MenuRepo : GenericRepo<Menus>, IMenuRepo
    {
        private readonly FoodOrderingDbContext _context;
        public MenuRepo(FoodOrderingDbContext context) : base(context) {
            _context = context;
        }

        public async Task<Menus?> GetMenuWithCategoryAsync(Guid id)
        {
            return await _context.Menus.Include(m => m.Categories).FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
