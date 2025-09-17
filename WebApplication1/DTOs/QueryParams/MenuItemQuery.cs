using System.Text.Json.Serialization;

namespace Food_Ordering.DTOs.QueryParams
{
    public class MenuItemQuery
    {
        [JsonPropertyName("page")]
        public int Page { get; set; }

        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }

        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("category")]
        public string? Category { get; set; }

        [JsonPropertyName("isAvailable")]
        public bool? IsAvailable { get; set; }
    }
}
