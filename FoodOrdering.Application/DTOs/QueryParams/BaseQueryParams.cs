using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.QueryParams
{
    public abstract class BaseQueryParams
    {
        [JsonProperty("page")]   
        
        public int Page { get; set; }

        [JsonProperty("pageSize")]
        public int PageSize { get; set; }

        [JsonProperty("search")]
        public string? Search { get; set; }
    }
}
