using Food_Ordering.DTOs.Response;
using Net.payOS.Types;

namespace Food_Ordering.Services.Payment
{
    public interface IPayOSService
    {
        public Task<dynamic> CreatePaymentLink(int orderCode, int amount, List<ItemData> items);
        public Task<string> ConfirmWebHook();
        public Task<string> CallBack(HttpRequest request);
    }
}
