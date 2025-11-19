using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using FoodOrdering.Infrastructure.Repository;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repositories
{
    public class RatingRepo : GenericRepo<Rating>, IRatingRepo
    {
        private readonly FoodOrderingDbContext _context;
        public RatingRepo(FoodOrderingDbContext context) : base(context) {
          _context = context;
        }

        public async Task<double> GetAverageRating(Guid menuId)
        {
            var ratings = _context.Rating.Where(r => r.MenuId == menuId);

            if (!await ratings.AnyAsync())
                return 0; 

            var avg = await ratings.AverageAsync(r => r.Stars);

            return avg;
        }
    }
}
