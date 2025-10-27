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
        public Task AddAsync(VoucherRequest request);
        public Task UpdateAsync(Guid id, VoucherRequest request);
        public Task DeleteAsync(Guid id);
        public Task<VoucherDTO> ValidateVoucherAsync(ValidateVoucherRequest request);
    }
}
