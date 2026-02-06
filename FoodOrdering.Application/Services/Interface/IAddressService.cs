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
        public Task SetAddressAsDefault(Guid addressId);
        public Task<IEnumerable<AddressDto>> GetAllByUserAsync(Guid userId);
        public Task AddAsync(AddressRequestDto request);
        public Task UpdateAsync(Guid addressId , AddressRequestDto request);
        public Task DeleteAsync(Guid addressId);
    }
}
