using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.QueryParams
{
    public class MenuParams
    {
        [JsonProperty("page")]           
        public int Page { get; set; }
        [JsonProperty("pageSize")]
        public int PageSize { get; set; }
        [JsonProperty("name")]
        public string? Name { get; set; }
        [JsonProperty("category")]
        public string? Category { get; set; }
        [JsonProperty("isAvailable ")]
        public bool? IsAvailable { get; set; }
        [JsonProperty("sortBy")]
        public string? SortBy { get; set; }
        [JsonProperty("sortOrder")]
        public string? SortOrder { get; set; }
    }
}
