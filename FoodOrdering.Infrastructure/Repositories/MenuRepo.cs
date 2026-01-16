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
    public class MenuRepo : GenericRepo<Menu>, IMenuRepo
    {
        private readonly FoodOrderingDbContext _context;
        public MenuRepo(FoodOrderingDbContext context) : base(context) {
            _context = context;
        }

        public async Task<Menu?> GetMenuWithCategoryAsync(Guid id)
        {
            return await _context.Menu
                .Include(m => m.Categories)
                .Include(m => m.Ratings)
                .FirstOrDefaultAsync(m => m.Id == id);
        }
    }
}
