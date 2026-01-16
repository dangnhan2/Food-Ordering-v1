using FoodOrdering.Application.DTOs.Request;
using FoodOrdering.Application.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Services.Payment
{
    public interface IPayOsService
    {
        public Task<PaymentOrderInfo> CreatePaymentLink(int amount, int orderCode, List<ItemData> data);
        public Task<string> ConfirmWebHook(WebHookUrlDto request);
        public Task<string> CallBack(HttpRequest request);
    }
}
