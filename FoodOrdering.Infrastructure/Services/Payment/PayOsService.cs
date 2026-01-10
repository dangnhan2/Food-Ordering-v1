using DotNetEnv;
using FoodOrdering.Application;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Services.Interface;
using FoodOrdering.Application.Services.Payment;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Options;
using FoodOrdering.Infrastructure.SignalR_Hub;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Services.Payment
{
    public class PayOsService : IPayOsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOS _payOS;
        private readonly INotificationSenderService _notificationSenderServer;
        private readonly PayOsOptions _options;
        public PayOsService(
            IUnitOfWork unitOfWork, 
            INotificationSenderService notificationSenderServer,
            PayOS payOS,
            IOptions<PayOsOptions> options) {
            _unitOfWork = unitOfWork;
            _payOS = payOS;
            _options = options.Value;         
            _notificationSenderServer = notificationSenderServer;
        }

        public async Task<ApiResponse<string>> CallBack(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            var rawJson = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(rawJson))
                return ApiResponse<string>.Fail("Empty body", StatusCodes.Status400BadRequest);

            // Parse JSON
            var root = JObject.Parse(rawJson);
            var signatureProvided = root["signature"]?.ToString();
            var data = root["data"] as JObject;

            if (string.IsNullOrEmpty(signatureProvided) || data == null)
                return ApiResponse<string>.Fail("Invalid payload", StatusCodes.Status400BadRequest);

            // Build transactionStr = key=value&key2=value2...
            var sorted = data.Properties().OrderBy(p => p.Name, StringComparer.Ordinal).ToList();

            var sb = new StringBuilder();

            for (int i = 0; i < sorted.Count; i++)
            {
                var prop = sorted[i];
                sb.Append(prop.Name).Append('=').Append(prop.Value.ToString());
                if (i < sorted.Count - 1) sb.Append('&');
            }
            var transactionStr = sb.ToString();

            // Compute HMAC SHA256
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_options.ChecksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(transactionStr));
            var signatureComputed = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            if (!string.Equals(signatureProvided, signatureComputed, StringComparison.OrdinalIgnoreCase))            
               return ApiResponse<string>.Fail("Invalid signature", StatusCodes.Status401Unauthorized);
            
            // check order if success 
            var code = data["orderCode"].ToObject<int>();
            if (code == null)
                return ApiResponse<string>.Fail("Mã đơn hàng không hợp lệ", StatusCodes.Status400BadRequest);

            var order = await _unitOfWork.Order.GetOrderByOrderCode(code);

            if (order == null)            
                return ApiResponse<string>.Fail("Không tìm thấy order", StatusCodes.Status404NotFound);

            // Update status after order is paid
            order.Status = Food_Ordering.Models.Enum.OrderStatus.Paid;

            // Update sold quantity of each menu in paid order
            foreach(var menu in order.OrderMenus)
            {
                var item = await _unitOfWork.Menu.GetByIdAsync(menu.MenuId);

                if (item != null)
                item.SoldQuantity = item.SoldQuantity + menu.Quantity;
            }
            
            _unitOfWork.Order.Update(order);
            await _notificationSenderServer.NotifyAdminAsync(order.TransactionId, order.TotalAmount);
            await _unitOfWork.SaveChangeAsync();

            return ApiResponse<string>.Success("Webhook processed successfully", "", StatusCodes.Status200OK);
        }

        public async Task<string> ConfirmWebHook(string url)
        {
            var result =await _payOS.confirmWebhook(url);
            return result;
        }

        public async Task<dynamic> CreatePaymentLink(int amount, int orderCode, List<ItemData> data)
        {
            var returnUrl = _options.ReturnUrl;
            var cancelUrl = _options.CancelUrl;

            var paymentLinkRequest = new PaymentData(
                 orderCode : orderCode,
                 amount : amount,
                 description : "Thanh toán đơn hàng",
                 items: data,
                 expiredAt: (int) DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds(),
                 returnUrl: $"{returnUrl}checkout/success?orderCode=${orderCode}&paymentMethod=QR",
                 cancelUrl : cancelUrl
            );

            var response = await _payOS.createPaymentLink(paymentLinkRequest);

            return response;
        }
    }
}
