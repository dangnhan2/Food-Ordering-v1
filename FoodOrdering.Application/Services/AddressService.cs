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
        public readonly IUnitOfWork _unitOfWork; 

        public AddressService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ApiResponse<Addresses>> AddAsync(AddressRequest request)
        {
            Addresses address = new Addresses
            {              
                Address = request.Address,
                UserId = request.UserId,
            };

            await _unitOfWork.Address.AddAsync(address);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<Addresses>.Success("Thêm địa chỉ thành công", address, StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<Addresses>> DeleteAsync(Guid id)
        {
           var address = await _unitOfWork.Address.GetByIdAsync(id);

            if (address == null)
                return ApiResponse<Addresses>.Fail("Không tìm thấy địa chỉ", StatusCodes.Status404NotFound);

            _unitOfWork.Address.Remove(address);
            await _unitOfWork.SaveChangeAsync();
            return ApiResponse<Addresses>.Success("Xóa địa chỉ thành công", address, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<IEnumerable<AddressDto>>> GetAllByUserAsync(Guid id)
        {
            var addresses = _unitOfWork.Address.GetAll().Where(a => a.UserId == id);

            var addressesToDto = await addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                Address = a.Address
            }).ToListAsync();

            return ApiResponse<IEnumerable<AddressDto>>.Success("Lấy dữ liệu thành công", addressesToDto, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<Addresses>> UpdateAsync(Guid id, AddressRequest request)
        {
            var address = await _unitOfWork.Address.GetByIdAsync(id);

            if (address == null)
                return ApiResponse<Addresses>.Fail("Không tìm thấy địa chỉ", StatusCodes.Status404NotFound);

            address.Address = request.Address;
            address.UserId = request.UserId;

            _unitOfWork.Address.Update(address);
            await _unitOfWork.SaveChangeAsync();
            return ApiResponse<Addresses>.Success("Cập nhật địa chỉ thành công", address, StatusCodes.Status200OK);
        }
    }
}
