using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.QueryParams
{
    public class RatingParams : BaseQueryParams
    {

        [JsonPropertyName("stars")]
        public int? Stars { get; set; }
    }
}
