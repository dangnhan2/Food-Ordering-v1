using DotNetEnv;
using Food_Ordering.DTOs.Response;
using Net.payOS;
using Net.payOS.Types;
using System.Threading.Tasks;

namespace Food_Ordering.Services.Payment
{
    public class PayOSService : IPayOSService
    {
        private readonly PayOS _payOS;
        private readonly string _webHookUrl;
        private const string returnUrl = "Return";
        private const string cancelUrl = "Cancel";

        public PayOSService()
        {
            Env.Load();
            _payOS = new PayOS(
                Env.GetString("PAYOS_CLIENT_ID"), 
                Env.GetString("PAYOS_API_KEY"), 
                Env.GetString("PAYOS_CHECKSUM_KEY"));
            _webHookUrl = Env.GetString("PAYOS_CHECKSUM_KEY");
        }
        public Task<string> CallBack(HttpRequest request)
        {
            throw new NotImplementedException();
        }

        public async Task<string> ConfirmWebHook()
        {
            return await _payOS.confirmWebhook(_webHookUrl);
        }

        public async Task<dynamic> CreatePaymentLink(int orderCode, int amount, List<ItemData> items)
        {
            var paymentLinkRequest = new PaymentData(
                orderCode: orderCode,
                amount: amount,
                description: "Thanh toan don hang",
                items : items,
                returnUrl: returnUrl,
                cancelUrl: cancelUrl,
                expiredAt: (int)DateTimeOffset.UtcNow.AddMinutes(10).ToUnixTimeSeconds()
            );

            var response = await _payOS.createPaymentLink(paymentLinkRequest);

            return response;
        }
    }
}

//var paymentLinkRequest = new PaymentData(
//                orderCode: int.Parse(DateTimeOffset.Now.ToString("ffffff")),
//                amount: 2000,
//                description: "Thanh toan don hang",
//                items: [new("Mì tôm hảo hảo ly", 1, 2000)],
//                returnUrl: domain + "/success.html",
//                cancelUrl: domain + "/cancel.html"
//            );
//var response = await payOS.createPaymentLink(paymentLinkRequest);