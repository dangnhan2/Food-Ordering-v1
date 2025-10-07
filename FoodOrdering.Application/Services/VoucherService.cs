using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
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
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;

        public VoucherService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<ApiResponse<Voucher>> AddAsync(VoucherRequest request)
        {
            var voucher = new Voucher
            {
                Code = request.Code,
                Description = request.Description,
                DiscountType = request.DiscountType,
                DiscountValue = request.DiscountValue,
                StartDate = request.StartDate,
                EndDate = request.EndDate,
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

        public async Task<ApiResponse<PagingReponse<VoucherDTO>>> GetAllAsync(VoucherParams voucherParams)
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

        public async Task<ApiResponse<Voucher>> UpdateAsync(Guid id, VoucherRequest request)
        {
            var existVoucher = await _unitOfWork.Voucher.GetByIdAsync(id);

            if (existVoucher == null)
                return ApiResponse<Voucher>.Fail("Không tìm thấy voucher", StatusCodes.Status404NotFound);

            existVoucher.Code = request.Code;
            existVoucher.Description = request.Description;
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
    }
}
