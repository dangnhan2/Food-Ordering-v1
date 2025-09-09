using Food_Ordering.Models.Enum;

namespace Food_Ordering.Models
{
    public class Payments
    {
        public Guid Id { get; set; }
        public Orders Orders { get; set; }
        public Guid OrdersId { get; set; }
        public double Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public DateTime PaidAt { get; set; }
    }
}
