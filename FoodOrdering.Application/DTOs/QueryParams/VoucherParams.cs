using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.QueryParams
{
    public class VoucherParams
    {
        [JsonPropertyName("page")]           
        public int Page { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
      
        [JsonPropertyName("search")]
        public string? Search { get; set; }

        [JsonPropertyName("startDate")]
        public DateTime? StartDate { get; set; }
        [JsonPropertyName("endDate")]
        public DateTime? EndDate { get; set; }
        [JsonPropertyName("sortBy")]
        public string? SortBy { get; set; }
        [JsonPropertyName("sortOrder")]
        public string? SortOrder { get; set; }
    }
}
