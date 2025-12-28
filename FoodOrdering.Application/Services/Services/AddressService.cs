using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Repositories.Caching;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace FoodOrdering.Application.Services.Services
{
    public class AddressService : IAddressService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICachingService _cacheService;
        
        public AddressService(IUnitOfWork unitOfWork, ICachingService cacheService)
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
        }

        public async Task AddAsync(AddressRequestDto request)
        {   
            var result = await new AddressValidator().ValidateAsync(request);
            if (!result.IsValid)           
              throw new ValidationDictionaryException(result.ToDictionary());

            var newAddress = MappingAddress(request);

            await _unitOfWork.Address.AddAsync(newAddress);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(CacheKeys.UserAddresses(newAddress.UserId));
        }

        public async Task DeleteAsync(Guid id)
        {
            var address = await _unitOfWork.Address.GetByIdAsync(id);
            if (address == null)
                throw new KeyNotFoundException("Địa chỉ không tồn tại");
            _unitOfWork.Address.Remove(address);
            await _unitOfWork.SaveChangeAsync();
            await _cacheService.RemoveAsync(CacheKeys.UserAddresses(address.UserId));
        }

        public async Task<IEnumerable<AddressDto>> GetAllByUserAsync(Guid id)
        {
            string cacheKey = CacheKeys.UserAddresses(id);
            var cacheAddresses = await _cacheService.GetAsync<IEnumerable<AddressDto>>(cacheKey);
            if (cacheAddresses != null)
                return cacheAddresses;

            var addresses = _unitOfWork.Address.GetAll().Where(a => a.UserId == id);

            var addressesToDto = await addresses
                .Select(a => new AddressDto(a))
                .AsNoTracking()
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, addressesToDto, TimeSpan.FromHours(1));

            return addressesToDto;
        }

        public async Task UpdateAsync(Guid id, AddressRequestDto request)
        {   
            var result = await new AddressValidator().ValidateAsync(request);
            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());
            var address = await _unitOfWork.Address.GetByIdAsync(id);

            if (address == null)
                throw new KeyNotFoundException("Địa chỉ không tồn tại");

            address.FullName = request.FullName;
            address.PhoneNumber = request.PhoneNumber;
            address.AddressName = request.Address;
            address.UserId = request.UserId;

            _unitOfWork.Address.Update(address);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(CacheKeys.UserAddresses(address.UserId));
        }

        private Address MappingAddress(AddressRequestDto request)
        {
            Address address = new Address
            {
                FullName = request.FullName,
                PhoneNumber = request.PhoneNumber,
                AddressName = request.Address,
                UserId = request.UserId,
            };

            return address;
        }
    }
}
