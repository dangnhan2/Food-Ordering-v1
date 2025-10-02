using FoodOrdering.Application.DTOs.Response;
using Microsoft.AspNetCore.Http;
using Net.payOS.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.Payment
{
    public interface IPaymentGateway
    {
        public Task<dynamic> CreatePaymentLink(int amount, int orderCode, List<ItemData> data);
        public Task<string> ConfirmWebHook(string url);
        public Task<Result<string>> CallBack(HttpRequest request);
    }
}
