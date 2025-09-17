using System.Text.Json.Serialization;

namespace Food_Ordering.DTOs.QueryParams
{
    public class OrderQuery
    {
        [JsonPropertyName("page")]   
        public int Page { get; set; }
        [JsonPropertyName("pageSize")]
        public int PageSize { get; set; }
    }
}
