using FoodOrdering.Application;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
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
            var vouchers = _unitOfWork.Voucher.GetAll().Where(v => v.StartDate.Date == DateTime.UtcNow.Date && v.IsActive == false);

            foreach(var voucher in vouchers)
            {
                voucher.IsActive = true;
            }

            if (vouchers.Count() > 0)
            {
                await _unitOfWork.SaveChangeAsync();
            }
        }

        public async Task RetrieveVouchers_24hours()
        {

            var vouchers = _unitOfWork.Voucher.GetAll().Where(v => v.EndDate.Date == DateTime.UtcNow.Date && v.IsActive == true);

            foreach (var voucher in vouchers)
            {
                voucher.IsActive = false;
            }

            if (vouchers.Count() > 0)
            {
                await _unitOfWork.SaveChangeAsync();
            }
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
    }
}
