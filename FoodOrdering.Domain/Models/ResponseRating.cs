using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class ResponseRating
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid RatingId { get; set; }
        public Rating Rating { get; set; }
        public string Comment { get; set; }
        public DateTimeOffset ResponseAt { get; set; } = DateTimeOffset.UtcNow;
    }
}
