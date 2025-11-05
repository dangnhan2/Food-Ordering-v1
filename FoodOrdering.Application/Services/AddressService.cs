using FoodOrdering.Application.Caching;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        
        public AddressService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        // add new address by specific customer
        public async Task AddAsync(AddressRequest request)
        {   
            var result = await new AddressValidator().ValidateAsync(request);
            if (!result.IsValid)           
              throw new ValidationDictionaryException(result.ToDictionary());
            
            string cacheKey = $"user:{request.UserId}:addresses";

            Addresses address = new Addresses
            {   
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                UserId = request.UserId,
            };

            await _unitOfWork.Address.AddAsync(address);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(cacheKey);
        }

        // delete address by specific customer
        public async Task DeleteAsync(Guid id)
        {
            var address = await _unitOfWork.Address.GetByIdAsync(id);
            if (address == null)
                throw new KeyNotFoundException(nameof(address));

            string cacheKey = $"user:{address.UserId}:addresses";

            _unitOfWork.Address.Remove(address);
            await _unitOfWork.SaveChangeAsync();
            await _cacheService.RemoveAsync(cacheKey);
        }

        // get addresses by specific customer
        public async Task<IEnumerable<AddressDto>> GetAllByUserAsync(Guid id)
        {
            string cacheKey = $"user:{id}:addresses";
            var cacheAddresses = await _cacheService.GetAsync<IEnumerable<AddressDto>>(cacheKey);
            if (cacheAddresses != null)
                return cacheAddresses;

            var addresses = _unitOfWork.Address.GetAll().Where(a => a.UserId == id);

            var addressesToDto = await addresses.Select(a => new AddressDto(a)).AsNoTracking().ToListAsync();

            await _cacheService.SetAsync(cacheKey, addressesToDto, TimeSpan.FromHours(1));

            return addressesToDto;
        }

        // update address by specific customer
        public async Task UpdateAsync(Guid id, AddressRequest request)
        {
            var result = await new AddressValidator().ValidateAsync(request);
            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            string cacheKey = $"user:{request.UserId}:addresses";
            var address = await _unitOfWork.Address.GetByIdAsync(id);

            if (address == null)
                throw new KeyNotFoundException(nameof(address));

            address.FullName = request.FullName;
            address.PhoneNumber = request.PhoneNumber;
            address.Address = request.Address;
            address.UserId = request.UserId;

            _unitOfWork.Address.Update(address);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(cacheKey);
        }
    }
}
