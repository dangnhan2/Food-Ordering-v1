using DotNetEnv;
using FoodOrdering.Application;
using FoodOrdering.Application.DTOs.Response;
using FoodOrdering.Application.Payment;
using Microsoft.AspNetCore.Http;
using Net.payOS;
using Net.payOS.Types;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Payment
{
    public class PaymentGateway : IPaymentGateway
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly PayOS _payOS;
        private readonly string returnUrl;
        private readonly string cancelUrl;
        private readonly string _checksumKey;
        public PaymentGateway(IUnitOfWork unitOfWork) {
            Env.Load();
            _unitOfWork = unitOfWork;
            _payOS = new PayOS(
                Env.GetString("PAYOS_CLIENT_ID"),
                Env.GetString("PAYOS_API_KEY"),
                Env.GetString("PAYOS_CHECKSUM_KEY")
                );

            returnUrl = Env.GetString("RETURN_URL");
            cancelUrl = Env.GetString("CANCEL_URL");
            _checksumKey = Env.GetString("PAYOS_CHECKSUM_KEY");
        }

        public async Task<Result<string>> CallBack(HttpRequest request)
        {
            using var reader = new StreamReader(request.Body, Encoding.UTF8);
            var rawJson = await reader.ReadToEndAsync();

            if (string.IsNullOrWhiteSpace(rawJson))
                return Result<string>.Fail("Empty body", StatusCodes.Status400BadRequest);

            //Console.WriteLine("Webhook raw: " + rawJson);

            // Parse JSON
            var root = JObject.Parse(rawJson);
            var signatureProvided = root["signature"]?.ToString();
            var data = root["data"] as JObject;

            if (string.IsNullOrEmpty(signatureProvided) || data == null)
                return Result<string>.Fail("Invalid payload", StatusCodes.Status400BadRequest);

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
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(transactionStr));
            var signatureComputed = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            if (!string.Equals(signatureProvided, signatureComputed, StringComparison.OrdinalIgnoreCase))
            {
                return Result<string>.Fail("Invalid signature", StatusCodes.Status401Unauthorized);
            }

            // check order if success 
            var code = data["orderCode"].ToObject<int>();
            if (code == null)
                return Result<string>.Fail("Mã đơn hàng không hợp lệ", StatusCodes.Status400BadRequest);

            var order = await _unitOfWork.Order.GetOrderByOrderCode(code);

            if (order == null)            
                return Result<string>.Fail("Không tìm thấy order", StatusCodes.Status404NotFound);

            order.Status = Food_Ordering.Models.Enum.OrderStatus.Paid;

            _unitOfWork.Order.Update(order);
            await _unitOfWork.SaveChangeAsync();

            return Result<string>.Success("Webhook processed successfully", "", StatusCodes.Status200OK);
        }

        public async Task<string> ConfirmWebHook(string url)
        {
            var webhookUrl = await _payOS.confirmWebhook(url);
            return webhookUrl;
        }

        public async Task<dynamic> CreatePaymentLink(int amount, int orderCode, List<ItemData> data)
        {
            var paymentLinkRequest = new PaymentData(
                 orderCode : orderCode,
                 amount : amount,
                 description : "Thanh toán đơn hàng",
                 items : data,
                 expiredAt: (int)DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeMilliseconds(),
                 returnUrl: returnUrl,
                 cancelUrl : cancelUrl
            );

            var response = await _payOS.createPaymentLink(paymentLinkRequest);

            return response;
        }
    }
}
