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
        public Task<Result<PagingReponse<MenuDTO>>> GetAllAsync(MenuParams menuParams);
        public Task<Result<MenuDTO>> GetByIdAsync(Guid id);
        public Task<Result<Menus>> AddAsync(MenuRequest request);
        public Task<Result<Menus>> UpdateAsync(Guid id, MenuRequest request);
        public Task<Result<Menus>> DeleteAsync(Guid id);
    }
}
