using FoodOrdering.Application.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application
{
    public interface IUnitOfWork : IDisposable
    {
       IOrderRepo Order { get; }
       IMenuRepo Menu { get; }
       IUserRepo User { get; }
       ICategoryRepo Category { get; }
       IVoucherRepo Voucher { get; }
       IRefreshTokenRepo RefreshToken { get; }
       ICartRepo Cart { get; }
       IEmailOtpRepo EmailOtp { get; }
       IVoucherRedemptionRepo VoucherRedemption { get; }
       IAddressRepo Address { get; }
       INotificationRepo Notification { get; }
       IOrderMenuRepo OrderMenu { get; }
       IRatingRepo Rating { get; }  
       IResponseRating ResponseRating { get; }
       IAdvertisementRepo Advertisement { get; }
       Task SaveChangeAsync();
       Task BeginTransactionAsync();
       Task CommitTransactionAsync();
       Task RollbackTransactionAsync();
    }
}
