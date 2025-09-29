using Food_Ordering.Models;

namespace FoodOrdering.Domain.Models
{
    public class VoucherRedemptions
    {
        public Guid Id{ get; set; }
        public Guid VoucherID { get; set; }
        public Voucher Voucher { get; set; } = null!;
        public Guid UserID { get; set; }
        public User User { get; set; } = null!;
        public Guid OrderID { get; set; }
        public Orders Order { get; set; } = null!;
        public DateTime RedeemedAt { get; set; }
        public decimal AmountDiscount { get; set; }
        
    }
}
