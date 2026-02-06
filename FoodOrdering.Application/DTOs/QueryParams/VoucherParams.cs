using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.QueryParams
{
    public class VoucherParams : BaseQueryParams
    {
        [JsonPropertyName("from")]
        public DateTime? From { get; set; }

        [JsonPropertyName("to")]
        public DateTime? To { get; set; }
    }
}
