using FoodOrdering.Application;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repositories
{
    public class BackgroundJobScheduler : IBackgroundJobScheduler
    {
        private readonly IUnitOfWork _unitOfWork;
         
        public BackgroundJobScheduler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task DeleteExpiredCarts_3hours()
        {
            var carts = await _unitOfWork.Cart.FindAsync(c => c.CreatedAt.AddHours(3) < DateTime.UtcNow);

            if(carts.Count() > 0)
            {
                _unitOfWork.Cart.RemoveRange(carts);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task DeleteCancelledOrder_10days(Guid id)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(id);

            if(order != null && order.Status == Food_Ordering.Models.Enum.OrderStatus.Cancelled)
            {
                _unitOfWork.Order.Remove(order);
                await _unitOfWork.SaveChangeAsync();
            }          
        }

        public async Task DeleteExpiredOtp_5mins(Guid id)
        {
            var user = await _unitOfWork.User.GetUserContainsOtpAsync(id);
      
            if (user != null && !user.EmailConfirmed && user.EmailOtp.ExpiredAt < DateTime.UtcNow)
            {
                _unitOfWork.User.Remove(user);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task DeleteExpiredRefreshTokens_3months()
        {
            var tokens = await _unitOfWork.RefreshToken.FindAsync(t => t.ExpriedAt < DateTime.UtcNow);

            if(tokens.Count() > 0)
            {
                _unitOfWork.RefreshToken.RemoveRange(tokens);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task UpdateExpiredOrder_10mins(Guid id)
        {
            var order = await _unitOfWork.Order.GetByIdAsync(id);
            var voucherRedemption = await _unitOfWork.VoucherRedemption.GetByIdAsync(v => v.OrderID == id);
            var voucher = await _unitOfWork.Voucher.GetByIdAsync(voucherRedemption.VoucherID);

            if (order != null && order.ExpiredAt < DateTime.UtcNow && order.Status == Food_Ordering.Models.Enum.OrderStatus.Pending && voucherRedemption != null && voucher != null)
            {   
                // update order status if order not paid
                order.Status = Food_Ordering.Models.Enum.OrderStatus.Cancelled;

                // decrease used voucher if order not paid
                voucher.UsedCount -= 1; ;

                if (voucher.UsedCount < voucher.UsageLimit)
                    voucher.IsActive = true;
                // delete voucher redemption
                _unitOfWork.VoucherRedemption.Remove(voucherRedemption);
                _unitOfWork.Voucher.Update(voucher);
                _unitOfWork.Order.Update(order);
                await _unitOfWork.SaveChangeAsync();
            }

        }

        public async Task PublicVouchers_24hours()
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

            var vouchers = await _unitOfWork.Voucher.FindAsync(
                v => v.StartDate >= startUtc &&
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

        public async Task RetrieveVouchers_24hours()
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

            var vouchers = await _unitOfWork.Voucher.FindAsync(
                v => v.EndDate < todayUtc &&
                     v.IsActive
            );

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

        public async Task ResetVoucherRedemptions_24hours()
        {
            var voucherRedemptions = _unitOfWork.VoucherRedemption.GetAll();

            if (voucherRedemptions.Count() > 0)
            {
                _unitOfWork.VoucherRedemption.RemoveRange(voucherRedemptions);
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task DeleteNotifications_1month()
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
