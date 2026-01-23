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
            var menus = _unitOfWork.Menu.GetAll();

            menus = menus.Where(m => m.Name.Replace(" ", "").Contains(requestDto.Keyword.Replace(" ", "")));

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
