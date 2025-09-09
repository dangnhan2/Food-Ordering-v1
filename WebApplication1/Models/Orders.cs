using Food_Ordering.Models.Enum;

namespace Food_Ordering.Models
{
    public class Orders
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; }
        public double ToTalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public ICollection<OrderItems> Items { get; set; } = new List<OrderItems>();
    }
}
