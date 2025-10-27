using FoodOrdering.Application.Caching;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using RedLockNet.SERedis;
using StackExchange.Redis;
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
        private readonly ICacheService _cacheService;
        public VoucherService(IUnitOfWork unitOfWork, ICacheService cacheService)
        {
            _unitOfWork = unitOfWork;           
            _cacheService = cacheService;            
        }
        public async Task AddAsync(VoucherRequest request)
        {
            var cacheKey = $"voucher:active";

            var result = await new VoucherValidator().ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

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

            if (voucher.IsActive)
                await _cacheService.RemoveAsync(cacheKey);
        }

        public async Task DeleteAsync(Guid id)
        {
            string cacheKey = $"voucher:active";
            var existVoucher = await _unitOfWork.Voucher.GetByIdAsync(id);

            if (existVoucher == null)
                throw new KeyNotFoundException(nameof(existVoucher));               

            //if (existVoucher.IsActive)
            //    throw new 
             
            _unitOfWork.Voucher.Remove(existVoucher);
            await _unitOfWork.SaveChangeAsync();

            await _cacheService.RemoveAsync(cacheKey);
        }

        public async Task<PagingReponse<VoucherDTO>> GetAllByAdminAsync(VoucherParams voucherParams)
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

            IEnumerable<VoucherDTO> voucherToDTO;

            if (voucherParams.Page == 0 || voucherParams.PageSize == 0)
            {
               voucherToDTO = await vouchers
                    .Select(v => new VoucherDTO(v))
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                voucherToDTO = await vouchers
                    .Select(v => new VoucherDTO(v))
                    .Paging(voucherParams.Page, voucherParams.PageSize)
                    .AsNoTracking()
                    .ToListAsync();
            }

            return new PagingReponse<VoucherDTO>(voucherParams.Page, voucherParams.PageSize, vouchers.Count(), voucherToDTO);                 
        }

        public async Task<IEnumerable<VoucherDTO>> GetAllByCustomerAsync()
        {
            string cacheKey = $"voucher:active";
            var cacheVouchers = await _cacheService.GetAsync<IEnumerable<VoucherDTO>>(cacheKey);
            if (cacheVouchers != null)
                return cacheVouchers;

            var vouchers = await _unitOfWork.Voucher.FindAsync(v => v.IsActive);

            var vouchersToDTO = vouchers.Select(v => new VoucherDTO(v)).ToList();

            await _cacheService.SetAsync(cacheKey, vouchersToDTO, TimeSpan.FromMinutes(30));

            return vouchersToDTO;
        }      

        public async Task UpdateAsync(Guid id, VoucherRequest request)
        {
            string cacheKey = $"voucher:active";
            var result = await new VoucherValidator().ValidateAsync(request);

            if (!result.IsValid)           
                throw new ValidationDictionaryException(result.ToDictionary());
            

            var existVoucher = await _unitOfWork.Voucher.GetByIdAsync(id);

            if (existVoucher == null)
                throw new KeyNotFoundException(nameof(existVoucher));

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

            if (existVoucher.IsActive)
                await _cacheService.RemoveAsync(cacheKey);
        }

        public async Task<VoucherDTO> ValidateVoucherAsync(ValidateVoucherRequest request)
        {
            var voucher = await _unitOfWork.Voucher.GetByIdAsync(
                v => v.Id == request.VoucherId
                && v.StartDate <= DateTime.UtcNow
                && v.EndDate >= DateTime.UtcNow
                && v.UsedCount < v.UsageLimit
                && v.IsActive);

            if (voucher == null)
                throw new KeyNotFoundException(nameof(voucher));
             
            var todayCount = await _unitOfWork.VoucherRedemption.TodayCountAsync(request.UserId, voucher.Id);
            // check if user already used this voucher in the same day
            if (todayCount > 1)
                throw new InvalidDataException("Bạn đã sử dụng voucher này hôm nay rồi");

            if (voucher.MinOrderAmount > request.TotalAmount)
                throw new InvalidDataException($"Đơn hàng phải đạt giá trị tối thiểu {voucher.MinOrderAmount}");
               
            return new VoucherDTO(voucher);
        }
    }
}
