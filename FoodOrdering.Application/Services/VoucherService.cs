using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VoucherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<Voucher>> AddAsync(VoucherRequest request)
        {           
            var result = await new VoucherValidator().ValidateAsync(request);

            if (!result.IsValid)            
                return ApiResponse<Voucher>.Fail(result.ToDictionary(), StatusCodes.Status400BadRequest);


            var voucher = new Voucher
            {
                Code = request.Code,
                Description = $"Hạn sử dụng {request.StartDate} đến ngày {request.EndDate}",
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                StartDate = request.StartDate,
                EndDate = request.StartDate,
                MaxDiscount = request.MaxDiscount,
                MinOrderAmount = request.MinOrderAmount,
                PerUserLimit = request.PerUserLimit,
                UsageLimit = request.UsageLimit,
                UsedCount = 0,
                IsActive = request.IsActive,
            };
          
            await _unitOfWork.Voucher.AddAsync(voucher);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<Voucher>.Success("Tạo voucher mới thành công", voucher, StatusCodes.Status201Created);
        }

        public async Task<ApiResponse<Voucher>> DeleteAsync(Guid id)
        {
            var existVoucher = await _unitOfWork.Voucher.GetByIdAsync(id);

            if (existVoucher == null)
                return ApiResponse<Voucher>.Fail("Không tìm thấy voucher", StatusCodes.Status404NotFound);

            _unitOfWork.Voucher.Remove(existVoucher);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<Voucher>.Success("Xóa voucher thành công", existVoucher, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<PagingReponse<VoucherDTO>>> GetAllByAdminAsync(VoucherParams voucherParams)
        {
            var vouchers = _unitOfWork.Voucher.GetAll();

            if (voucherParams.StartDate.HasValue)
                vouchers = vouchers.Where(v => v.StartDate == voucherParams.StartDate.Value);
            if (voucherParams.EndDate.HasValue)
                vouchers = vouchers.Where(v => v.EndDate == voucherParams.EndDate.Value);

            if (!string.IsNullOrEmpty(voucherParams.SortBy))
            {   
                var sortBy = voucherParams.SortBy.ToLower();
                var sortOrder = voucherParams.SortOrder?.ToLower() ?? "asc";

                vouchers = sortBy
                switch
                {
                    "usageLimit" => sortOrder == "desc" ? vouchers.OrderByDescending(v => v.UsageLimit) : vouchers.OrderBy(v => v.UsageLimit),
                    "usedCount" => sortOrder == "desc" ? vouchers.OrderByDescending(v => v.UsedCount) : vouchers.OrderBy(v => v.UsedCount)
                };
            }

            var voucherToDTO = await vouchers.Select(v => new VoucherDTO
            {
                Id = v.Id,
                Code = v.Code,
                Description = v.Description,
                DiscountType = v.DiscountType,
                DiscountValue = v.DiscountValue,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                IsActive = v.IsActive,
                MaxDiscount = v.MaxDiscount,
                MinOrderAmount = v.MinOrderAmount,
                PerUserLimit = v.PerUserLimit,
                UsageLimit = v.UsageLimit,
                UsedCount = v.UsedCount,
            }).Paging(voucherParams.Page, voucherParams.PageSize).ToListAsync();

            return ApiResponse<PagingReponse<VoucherDTO>>.Success("Lấy dữ liệu thành công",
                new PagingReponse<VoucherDTO>(voucherParams.Page, voucherParams.PageSize, vouchers.Count(), voucherToDTO),
                StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<IEnumerable<VoucherDTO>>> GetAllByCustomerAsync()
        {
            var vouchers = await _unitOfWork.Voucher.FindAsync(v => v.IsActive);

            var vouchersToDTO = vouchers.Select(v => new VoucherDTO
            {
                Id = v.Id,
                Code = v.Code,
                Description = v.Description,
                DiscountType = v.DiscountType,
                DiscountValue = v.DiscountValue,
                StartDate = v.StartDate,
                EndDate = v.EndDate,
                IsActive = v.IsActive,
                MaxDiscount = v.MaxDiscount,
                MinOrderAmount = v.MinOrderAmount,
                PerUserLimit = v.PerUserLimit,
                UsageLimit = v.UsageLimit,
                UsedCount = v.UsedCount,
            });

            return ApiResponse<IEnumerable<VoucherDTO>>.Success("Lấy dữ liệu thành công", vouchersToDTO, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<Voucher>> UpdateAsync(Guid id, VoucherRequest request)
        {
            var result = await new VoucherValidator().ValidateAsync(request);

            if (!result.IsValid)           
                return ApiResponse<Voucher>.Fail(result.ToDictionary(), StatusCodes.Status400BadRequest);
            

            var existVoucher = await _unitOfWork.Voucher.GetByIdAsync(id);

            if (existVoucher == null)
                return ApiResponse<Voucher>.Fail("Không tìm thấy voucher", StatusCodes.Status404NotFound);

            existVoucher.Code = request.Code;
            existVoucher.Description = $"Hạn sử dụng {request.StartDate} đến ngày {request.EndDate}";
            existVoucher.DiscountType = request.DiscountType;
            existVoucher.DiscountValue = request.DiscountValue;
            existVoucher.StartDate = request.StartDate;
            existVoucher.EndDate = request.EndDate;
            existVoucher.MaxDiscount = request.MaxDiscount;
            existVoucher.MinOrderAmount = request.MinOrderAmount;
            existVoucher.PerUserLimit = request.PerUserLimit;
            existVoucher.UsedCount = 0;
            existVoucher.UsageLimit = request.UsageLimit;
            existVoucher.IsActive = request.IsActive;

            _unitOfWork.Voucher.Update(existVoucher);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<Voucher>.Success("Cập nhật voucher thành công", existVoucher, StatusCodes.Status200OK);
        }

        public async Task<ApiResponse<VoucherDTO>> ValidateVoucherAsync(ValidateVoucherRequest request)
        {
            var voucher = await _unitOfWork.Voucher.GetByIdAsync(
                v => v.Id == request.VoucherId
                && v.StartDate <= DateTime.UtcNow
                && v.EndDate >= DateTime.UtcNow
                && v.UsedCount < v.UsageLimit);

            if (voucher == null)
                return ApiResponse<VoucherDTO>.Fail("Voucher không tồn tại hoặc đã hết hạn", StatusCodes.Status404NotFound);

            var todayCount = await _unitOfWork.VoucherRedemption.TodayCountAsync(request.UserId, voucher.Id);
            // check if user already used this voucher in the same day
            if (todayCount > 1)
                return ApiResponse<VoucherDTO>.Fail("Bạn đã sử dụng voucher này hôm nay rồi", StatusCodes.Status400BadRequest);

            if (voucher.MinOrderAmount > request.TotalAmount)
                return ApiResponse<VoucherDTO>.Fail($"Không thể sử dụng voucher vì đơn hàng chưa đạt mức thanh toán {voucher.MinOrderAmount}", StatusCodes.Status400BadRequest);

            var voucherToDto = new VoucherDTO
            {   
                Id = voucher.Id,
                Code = voucher.Code,
                Description = voucher.Description,
                DiscountType = voucher.DiscountType,
                DiscountValue = voucher.DiscountValue, 
                MaxDiscount = voucher.MaxDiscount,
                MinOrderAmount = voucher.MinOrderAmount,
                StartDate = voucher.StartDate,
                EndDate = voucher.EndDate,
                PerUserLimit = voucher.PerUserLimit,
                UsageLimit = voucher.UsageLimit,
                UsedCount = voucher.UsedCount,
                IsActive = voucher.IsActive
            };

            return ApiResponse<VoucherDTO>.Success("Voucher hợp lệ", voucherToDto, StatusCodes.Status200OK);
        }
    }
}
