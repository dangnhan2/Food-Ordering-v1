using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Interface
{
    public interface IAddressService
    {
        public Task<ApiResponse<IEnumerable<AddressDto>>> GetAllByUserAsync(Guid id);
        public Task<ApiResponse<Addresses>> AddAsync(AddressRequest request);
        public Task<ApiResponse<Addresses>> UpdateAsync(Guid id , AddressRequest request);
        public Task<ApiResponse<Addresses>> DeleteAsync(Guid id);
    }
}
