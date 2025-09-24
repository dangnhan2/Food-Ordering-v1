using Food_Ordering.Models.Enum;

namespace Food_Ordering.Models
{
    public class Orders
    {
        public Guid Id { get; set; }
        public User User { get; set; }
        public string UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime ExpiredAt { get; set; } = DateTime.UtcNow.AddMinutes(10);
        public OrderStatus Status { get; set; }
        public int ToTalAmount { get; set; }
        public string PaymentMethod { get; set; }
        public int TransactionId { get; set; }
        public ICollection<OrderItems> Items { get; set; } = new List<OrderItems>();
    }
}
