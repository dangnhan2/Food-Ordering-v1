using FoodOrdering.Application.Caching;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
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
        public async Task<ApiResponse<Addresses>> AddAsync(AddressRequest request)
        {
            string cacheKey = $"user:{request.UserId}:addresses";

            Addresses address = new Addresses
            {              
                Address = request.Address,
                UserId = request.UserId,
            };

            await _unitOfWork.Address.AddAsync(address);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(cacheKey);

            return ApiResponse<Addresses>.Success("Thêm địa chỉ thành công", address, StatusCodes.Status201Created);
        }

        // delete address by specific customer
        public async Task<ApiResponse<Addresses>> DeleteAsync(Guid id)
        {
            var address = await _unitOfWork.Address.GetByIdAsync(id);
            if (address == null)
                return ApiResponse<Addresses>.Fail("Không tìm thấy địa chỉ", StatusCodes.Status404NotFound);

            string cacheKey = $"user:{address.UserId}:addresses";

            _unitOfWork.Address.Remove(address);
            await _unitOfWork.SaveChangeAsync();
            await _cacheService.RemoveAsync(cacheKey);
            return ApiResponse<Addresses>.Success("Xóa địa chỉ thành công", address, StatusCodes.Status200OK);
        }

        // get addresses by specific customer
        public async Task<ApiResponse<IEnumerable<AddressDto>>> GetAllByUserAsync(Guid id)
        {
            string cacheKey = $"user:{id}:addresses";
            var cacheAddresses = await _cacheService.GetAsync<IEnumerable<AddressDto>>(cacheKey);
            if (cacheAddresses != null)
                return ApiResponse<IEnumerable<AddressDto>>.Success("Lấy dữ liệu thành công", cacheAddresses, StatusCodes.Status200OK);

            var addresses = _unitOfWork.Address.GetAll().Where(a => a.UserId == id);

            var addressesToDto = await addresses.Select(a => new AddressDto(a)).AsNoTracking().ToListAsync();

            await _cacheService.SetAsync(cacheKey, addressesToDto, TimeSpan.FromHours(1));

            return ApiResponse<IEnumerable<AddressDto>>.Success("Lấy dữ liệu thành công", addressesToDto, StatusCodes.Status200OK);
        }

        // update address by specific customer
        public async Task<ApiResponse<Addresses>> UpdateAsync(Guid id, AddressRequest request)
        {
            string cacheKey = $"user:{request.UserId}:addresses";
            var address = await _unitOfWork.Address.GetByIdAsync(id);

            if (address == null)
                return ApiResponse<Addresses>.Fail("Không tìm thấy địa chỉ", StatusCodes.Status404NotFound);

            address.Address = request.Address;
            address.UserId = request.UserId;

            _unitOfWork.Address.Update(address);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(cacheKey);

            return ApiResponse<Addresses>.Success("Cập nhật địa chỉ thành công", address, StatusCodes.Status200OK);
        }
    }
}
