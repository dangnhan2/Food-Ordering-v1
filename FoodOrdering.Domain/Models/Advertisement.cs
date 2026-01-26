using FoodOrdering.Domain.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class Advertisement
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? BannerUrl { get; set; }
        public AdTargetType AdTargetType { get; set; }
        public string? TargetKey { get; set; }
        public DateTimeOffset StartAt { get; set; }
        public DateTimeOffset? EndAt { get; set; }
        public bool IsActive { get; set; }
    }
}
