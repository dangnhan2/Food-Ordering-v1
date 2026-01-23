using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
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

        public async Task<MenuDto?> GetMenuWithCategoryAsync(Guid id)
        {
            var menuDto = from m in _context.Menu
                          join c in _context.Category on m.CategoriesId equals c.Id
                          where m.Id == id
                          select new MenuDto
                          {
                              Id = m.Id,
                              Name = m.Name,
                              Category = c.Name,
                              Description = m.Description,
                              OriginalPrice = m.OriginalPrice,
                              DiscountPrice = m.DiscountPrice,
                              AverageRating = m.AverageRating,
                              ImageUrl = m.ImageUrl,
                              CreatedAt = m.CreatedAt,
                              SoldQuantity = m.SoldQuantity,
                              RatingCount = m.Ratings.Count(),
                              IsAvailable = m.IsAvailable,
                              IsOnSale = m.IsOnSale
                          };

            return await menuDto.FirstOrDefaultAsync();
        }
    }
}
