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
        public Task<IEnumerable<AddressDto>> GetAllByUserAsync(Guid id);
        public Task AddAsync(AddressRequestDto request);
        public Task UpdateAsync(Guid id , AddressRequestDto request);
        public Task DeleteAsync(Guid id);
    }
}
