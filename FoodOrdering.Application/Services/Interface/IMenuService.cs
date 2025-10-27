using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IMenuService
    {
        public Task<PagingReponse<MenuDto>> GetAllAsync(MenuParams menuParams);
        public Task<MenuDto> GetByIdAsync(Guid id);
        public Task AddAsync(MenuRequest request);
        public Task UpdateAsync(Guid id, MenuRequest request);
        public Task DeleteAsync(Guid id);
    }
}
