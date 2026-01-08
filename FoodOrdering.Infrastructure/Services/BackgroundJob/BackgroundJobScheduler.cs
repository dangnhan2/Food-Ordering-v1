using FoodOrdering.Application;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace FoodOrdering.Infrastructure.Services.BackgroundJob
{
    public class BackgroundJobScheduler : IBackgroundJobScheduler
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly UserManager<User> _userManager;
         
        public BackgroundJobScheduler(IUnitOfWork unitOfWork, UserManager<User> userManager)
        {
            _unitOfWork = unitOfWork;
            _userManager = userManager;
        }

        public async Task RecurringDeleteExpiredOtpJob_5mins()
        {
            var expiredOtps = _unitOfWork.EmailOtp
                .GetAll()
                .Where(otp => otp.ExpiredAt < DateTime.UtcNow);
      
            _unitOfWork.EmailOtp.RemoveRange(expiredOtps);
            await _unitOfWork.SaveChangeAsync();            
        }

        public async Task ScheduleUpdateExpiredOrderJob_10mins(Guid id)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException(nameof(order));

            var voucherRedemption = await _unitOfWork.VoucherRedemption.GetByIdAsync(v => v.OrderID == id);

            if (voucherRedemption != null)
            {
                var voucher = await _unitOfWork.Voucher.GetByIdAsync(voucherRedemption.VoucherID);

                if (voucher != null)
                {
                    // update order status if order not paid
                    order.Status = Food_Ordering.Models.Enum.OrderStatus.Cancelled;

                    // decrease used voucher if order not paid
                    voucher.UsedCount -= 1; ;

                    if (voucher.UsedCount - 1 < voucher.UsageLimit)
                        voucher.IsActive = true;
                    // delete voucher redemption
                    _unitOfWork.VoucherRedemption.Remove(voucherRedemption);
                    _unitOfWork.Voucher.Update(voucher);
                }              
            }
            _unitOfWork.Order.Update(order);
            await _unitOfWork.SaveChangeAsync();        
        }

        public async Task RecurringDeleteExpiredCartsJob_3hours()
        {
            var carts = _unitOfWork.Cart
                .GetAll()
                .Where(c => c.CreatedAt.AddHours(3) < DateTime.UtcNow);

            if(carts.Count() > 0)
            {
                _unitOfWork.Cart.RemoveRange(carts);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task RecurringCancelledOrderJob_1month(Guid id)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(id);

            if(order != null && order.Status == Food_Ordering.Models.Enum.OrderStatus.Cancelled)
            {
                _unitOfWork.Order.Remove(order);
                await _unitOfWork.SaveChangeAsync();
            }          
        }

        public async Task RecurringDeleteExpiredRefreshTokensJob_3months()
        {
            var tokens = _unitOfWork.RefreshToken
                .GetAll()
                .Where(t => t.ExpriedAt < DateTime.UtcNow);
               

            if(tokens.Count() > 0)
            {
                _unitOfWork.RefreshToken.RemoveRange(tokens);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task RecurringPublicVouchersJob_24hours()
        {
            Log.Information("Starting public voucher...");

            // Lấy timezone Việt Nam
            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

            // Lấy thời điểm hiện tại theo giờ Việt Nam
            var nowInVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            // Lấy 00:00 hôm nay theo giờ VN
            var startOfTodayVn = nowInVn.Date;
            var startOfTomorrowVn = startOfTodayVn.AddDays(1);

            // Convert về UTC để so sánh trong DB (vì DB đang lưu UTC)
            var startUtc = new DateTimeOffset(startOfTodayVn, TimeSpan.FromHours(7)).ToUniversalTime();
            var endUtc = new DateTimeOffset(startOfTomorrowVn, TimeSpan.FromHours(7)).ToUniversalTime();

            Log.Information($"VN Range: {startOfTodayVn} -> {startOfTomorrowVn}");
            Log.Information($"UTC Range: {startUtc} -> {endUtc}");

            var vouchers = _unitOfWork.Voucher
                .GetAll()
                .Where(v => v.StartDate >= startUtc &&
                     v.StartDate < endUtc &&
                     !v.IsActive);

            Log.Information($"Voucher cần active: {vouchers.Count()}");

            if (vouchers.Count() > 0)
            {
                foreach (var voucher in vouchers)
                    voucher.IsActive = true;

                await _unitOfWork.SaveChangeAsync();
                Log.Information("✅ Đã publish voucher thành công.");
            }
            else
            {
                Log.Information("Không có voucher nào cần publish hôm nay.");
            }

            Log.Information("Finish publishing voucher");
        }

        public async Task RecurringRetrieveVouchersJob_24hours()
        {
            Log.Information("Starting retrieve voucher...");

            var vnTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Asia/Ho_Chi_Minh");

            // Giờ hiện tại VN
            var nowInVn = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, vnTimeZone);

            // 00:00 hôm nay và hôm sau theo VN
            var startOfTodayVn = nowInVn.Date;
            var startOfTomorrowVn = startOfTodayVn.AddDays(1);

            // Convert về UTC để query DB (StartDate & EndDate trong DB là UTC)
            var todayUtc = new DateTimeOffset(startOfTodayVn, TimeSpan.FromHours(7)).ToUniversalTime();
            var tomorrowUtc = new DateTimeOffset(startOfTomorrowVn, TimeSpan.FromHours(7)).ToUniversalTime();

            Log.Information($"VN Range Ended: < {todayUtc} (UTC end cutoff)");

            var vouchers = _unitOfWork.Voucher
                .GetAll()
                .Where(v => v.EndDate < todayUtc && v.IsActive);

            Log.Information($"Voucher cần thu hồi: {vouchers.Count()}");

            if (vouchers.Count() > 0)
            {
                foreach (var voucher in vouchers)
                    voucher.IsActive = false;

                await _unitOfWork.SaveChangeAsync();
                Log.Information("✅ Đã thu hồi voucher hết hạn.");
            }
            else
            {
                Log.Information("Không có voucher nào hết hạn hôm nay.");
            }

            Log.Information("Finish retrieving voucher");
        }

        public async Task RecurringResetVoucherRedemptionsJob_24hours()
        {
            var voucherRedemptions = _unitOfWork.VoucherRedemption.GetAll();

            if (voucherRedemptions.Count() > 0)
            {
                _unitOfWork.VoucherRedemption.RemoveRange(voucherRedemptions);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task RecurringDeleteNotificationsJob_1month()
        {
            DateTime todayUtc = DateTime.UtcNow.Date;
            DateTime tomorrowUtc = todayUtc.AddDays(1);

            var notifications = _unitOfWork.Notification.GetAll().Where(x => x.CreatedAt.Date.AddDays(30) >= todayUtc && x.CreatedAt.Date.AddDays(30) < tomorrowUtc);

            if (notifications.Count() > 0)
            {
                _unitOfWork.Notification.RemoveRange(notifications);
                await _unitOfWork.SaveChangeAsync();

            }
        }
    }
}
