using FoodOrdering.Application.DTOs.QueryParams;
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
    public interface IVoucherService
    {
        public Task<ApiResponse<PagingReponse<VoucherDTO>>> GetAllByAdminAsync(VoucherParams voucherParams);
        public Task<ApiResponse<IEnumerable<VoucherDTO>>> GetAllByCustomerAsync();
        public Task<ApiResponse<Voucher>> AddAsync(VoucherRequest request);
        public Task<ApiResponse<Voucher>> UpdateAsync(Guid id, VoucherRequest request);
        public Task<ApiResponse<Voucher>> DeleteAsync(Guid id);
        public Task<ApiResponse<Voucher>> ValidateVoucherAsync(ValidateVoucherRequest request);
    }
}
