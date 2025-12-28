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
        public Task<PagingReponse<VoucherDTO>> GetAllByAdminAsync(VoucherParams voucherParams);
        public Task<IEnumerable<VoucherDTO>> GetAllByCustomerAsync();
        public Task<VoucherDTO> GetByIdAsync(Guid id);
        public Task AddAsync(VoucherRequestDto request);
        public Task UpdateAsync(Guid id, VoucherRequestDto request);
        public Task DeleteAsync(Guid id);
        public Task<VoucherValidationDto> ValidateVoucherAsync(ValidateVoucherRequestDto request);
    }
}
