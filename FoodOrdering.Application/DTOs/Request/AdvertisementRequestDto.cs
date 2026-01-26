using FoodOrdering.Domain.Enum;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Application.DTOs.Request
{
    public class AdvertisementRequestDto
    {
        public string Title { get; set; }
        public IFormFile? BannerUrl { get; set; }
        public AdTargetType AdTargetType { get; set; }
        public string? TargetKey { get; set; }
        public int Priority { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public bool IsActive { get; set; }
    }
}
