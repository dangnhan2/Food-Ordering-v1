using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Services
{
    public class SearchService : ISearchService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SearchService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<MenuSearchDto>> SearchMenuAsync(SearchRequestDto requestDto)
        {
            var menus = _unitOfWork.Menu
                .GetAll()
                .Where(m => EF.Functions.ILike(EF.Functions.Unaccent(m.Name), "%" + EF.Functions.Unaccent(requestDto.Keyword) + "%"));

            var menusToDto = menus.Select(m => new MenuSearchDto
            {
                Id = m.Id,
                Name = m.Name,
                Price = m.IsOnSale ? m.DiscountPrice : m.OriginalPrice,
                ImageUrl = m.ImageUrl
            }).Take(5);

            return await menusToDto.ToListAsync();
        }
    }
}
