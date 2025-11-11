using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Repositories
{
    public interface IBackgroundJobScheduler
    {
        public Task DeleteExpiredOtp_5mins(Guid id);
        public Task DeleteCancelledOrder_30days(Guid id);
        public Task DeleteExpiredRefreshTokens_3months();
        public Task DeleteExpiredCarts_3hours();
        public Task UpdateExpiredOrder_10mins(Guid id);
        public Task PublicVouchers_24hours();
        public Task RetrieveVouchers_24hours();
        public Task ResetVoucherRedemptions_24hours();
        public Task DeleteNotifications_1month();
    }
}
