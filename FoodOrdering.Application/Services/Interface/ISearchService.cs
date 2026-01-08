using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface ISearchService
    {
        public Task<IEnumerable<MenuSearchDto>> SearchMenuAsync(SearchRequestDto requestDto);
    }
}
