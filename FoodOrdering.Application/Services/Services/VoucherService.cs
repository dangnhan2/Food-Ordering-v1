using FoodOrdering.Application.Contants;
using FoodOrdering.Application.DTOs.QueryParams;
using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Application.Repositories.Caching;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Validator;
using FoodOrdering.Domain.Models;
using Microsoft.EntityFrameworkCore;


namespace FoodOrdering.Application.Services.Services
{
    public class VoucherService : IVoucherService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICachingService _cacheService;
        private int TAX_RATE = 8;
        public VoucherService(IUnitOfWork unitOfWork, ICachingService cacheService)
        {
            _unitOfWork = unitOfWork;           
            _cacheService = cacheService;            
        }
        public async Task AddAsync(VoucherRequestDto request)
        {
            var result = await new VoucherValidator().ValidateAsync(request);

            if (!result.IsValid)
                throw new ValidationDictionaryException(result.ToDictionary());

            var newVoucher = MappingVoucher(request);
          
            await _unitOfWork.Voucher.AddAsync(newVoucher);
            await _unitOfWork.SaveChangeAsync();

            if (newVoucher.IsActive)
                await _cacheService.RemoveAsync(CacheKeys.VOUCHER_ACTIVE);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existVoucher = await _unitOfWork.Voucher.GetByIdAsync(id);

            if (existVoucher == null)
                throw new KeyNotFoundException("Mã giảm giá không tồn tại");

            if (existVoucher.IsActive)
                throw new InvalidOperationException("Mã giảm giá đang được áp dụng. Hãy cập nhật lại trước khi xóa");

            _unitOfWork.Voucher.Remove(existVoucher);
            await _unitOfWork.SaveChangeAsync();
            await _cacheService.RemoveAsync(CacheKeys.VoucherDetail(existVoucher.Id));

            if(existVoucher.IsActive)
              await _cacheService.RemoveAsync(CacheKeys.VOUCHER_ACTIVE);
        }

        public async Task<PagingReponse<VoucherDTO>> GetAllByAdminAsync(VoucherParams voucherParams)
        {          
            var vouchers = _unitOfWork.Voucher.GetAll();

            if (!string.IsNullOrEmpty(voucherParams.Search))
                vouchers = vouchers.Where(v => v.Code.ToUpper() == voucherParams.Search.ToUpper());

            if (voucherParams.StartDate.HasValue && voucherParams.EndDate.HasValue)
                vouchers = vouchers.Where(v => v.StartDate == voucherParams.StartDate.Value && v.EndDate == voucherParams.EndDate.Value);            

            if (!string.IsNullOrEmpty(voucherParams.SortBy))
            {
                var sortBy = voucherParams.SortBy.ToLower();
                var sortOrder = voucherParams.SortOrder?.ToLower() ?? "asc";

                vouchers = sortBy
                switch
                {
                    "usageLimit" =>
                    sortOrder == "desc"
                    ? vouchers.OrderByDescending(v => v.UsageLimit)
                    : vouchers.OrderBy(v => v.UsageLimit),

                    "usedCount" =>
                    sortOrder == "desc"
                    ? vouchers.OrderByDescending(v => v.UsedCount)
                    : vouchers.OrderBy(v => v.UsedCount),

                    _ => vouchers.OrderByDescending(v => v.StartDate)
                };
            }

            var voucherToDTO = vouchers
                    .Select(v => new VoucherDTO(v))
                    .AsNoTracking();

            if (voucherParams.Page != 0 && voucherParams.PageSize != 0)           
              voucherToDTO = voucherToDTO.Paging(voucherParams.Page, voucherParams.PageSize);
                      

            var response = new PagingReponse<VoucherDTO>(voucherParams.Page, voucherParams.PageSize, vouchers.Count(), await voucherToDTO.ToListAsync());
            return response; 
        }

        public async Task<IEnumerable<VoucherDTO>> GetAllByCustomerAsync()
        {
            string cacheKey = CacheKeys.VOUCHER_ACTIVE;
            var cacheVouchers = await _cacheService.GetAsync<IEnumerable<VoucherDTO>>(cacheKey);
            if (cacheVouchers != null)
                return cacheVouchers;

            var vouchers = _unitOfWork.Voucher.GetAll();

            var vouchersToDTO = await vouchers
                .Where(v => v.IsActive)
                .Select(v => new VoucherDTO(v))
                .AsNoTracking()
                .ToListAsync();

            await _cacheService.SetAsync(cacheKey, vouchersToDTO, TimeSpan.FromMinutes(30));

            return vouchersToDTO;
        }

        public async Task<VoucherDTO> GetByIdAsync(Guid id)
        {
            var cacheKey = CacheKeys.VoucherDetail(id);
            var cached = await _cacheService.GetAsync<VoucherDTO>(cacheKey);
            if (cached != null) return cached;

            var voucher = await _unitOfWork.Voucher.GetByIdAsync(id);

            if (voucher == null)
                throw new KeyNotFoundException("Mã giảm giá không tồn tại");
            var voucherToDto = new VoucherDTO(voucher);

            await _cacheService.SetAsync(cacheKey, voucherToDto, TimeSpan.FromHours(12));
            return voucherToDto;
        }

        public async Task UpdateAsync(Guid id, VoucherRequestDto request)
        {
            var result = await new VoucherValidator().ValidateAsync(request);

            if (!result.IsValid)           
                throw new ValidationDictionaryException(result.ToDictionary());
       
            var existVoucher = await _unitOfWork.Voucher.GetByIdAsync(id);

            if (existVoucher == null)
                throw new KeyNotFoundException("Mã giảm giá không tồn tại");

            existVoucher.Code = request.Code;
            existVoucher.Description = $"Hạn sử dụng {request.StartDate.FormatDateTimeOffset()} đến ngày {request.EndDate.FormatDateTimeOffset()}";
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
            await _cacheService.RemoveAsync(CacheKeys.VoucherDetail(existVoucher.Id));
            await _unitOfWork.SaveChangeAsync();

            if (existVoucher.IsActive)
                await _cacheService.RemoveAsync(CacheKeys.VOUCHER_ACTIVE);
        }

        public async Task<VoucherValidationDto> ValidateVoucherAsync(ValidateVoucherRequestDto request)
        {   
            var voucher = await _unitOfWork.Voucher.GetByIdAsync(
                v => v.Id == request.VoucherId
                && v.StartDate <= DateTime.UtcNow
                && v.EndDate >= DateTime.UtcNow
                && v.UsedCount < v.UsageLimit
                && v.IsActive);

            if (voucher == null)
                throw new KeyNotFoundException("Mã giảm giá không tồn tại");

            var cart = await _unitOfWork.Cart.GetCartByCustomerAsync(request.UserId);

            if (cart == null || cart.CartItems.Count == 0)
                throw new KeyNotFoundException("Giỏ hàng trống / không tồn tại");

            int subTotal = GetTotalAmount(cart.CartItems);
             
            // check if user already used this voucher in the same day
            var todayCount = await _unitOfWork.VoucherRedemption.TodayCountAsync(request.UserId, voucher.Id);
            if (todayCount >= 1)
                throw new InvalidDataException("Bạn đã sử dụng voucher này hôm nay rồi");

            if (voucher.MinOrderAmount > subTotal)
                throw new InvalidDataException($"Đơn hàng phải đạt giá trị tối thiểu {voucher.MinOrderAmount}");

            // calculate tax
            subTotal = subTotal + subTotal * TAX_RATE / 100;

            // calculate discount
            int discountAmount = subTotal * voucher.DiscountValue / 100;

            // check if discount amount is greater than max discount or not. 
            // Yes => assign discount amount to voucher's max discount
            // No => keep discount amount
            if (discountAmount > voucher.MaxDiscount)
                discountAmount = voucher.MaxDiscount;

            int totalAmount = subTotal - discountAmount;

            return new VoucherValidationDto(discountAmount, totalAmount);
        }

        private int GetTotalAmount(ICollection<CartItem> items)
        {   
            int subTotal = 0;
            foreach(var item in items)
            {
                subTotal += item.Quantity * item.UnitPrice;
            }
            return subTotal; 
        }

        private Voucher MappingVoucher(VoucherRequestDto request)
        {              
            var voucher = new Voucher
            {
                Code = request.Code,
                Description = $"Hạn sử dụng {request.StartDate.FormatDateTimeOffset()} đến ngày {request.EndDate.FormatDateTimeOffset()}",
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

            return voucher;
        }
    }
}
