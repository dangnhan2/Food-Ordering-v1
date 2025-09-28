using Food_Ordering.Models;

namespace FoodOrdering.Domain.Models
{
    public class VoucherRedemptions
    {
        public Guid Id{ get; set; }
        public Guid VoucherID { get; set; }
        public Voucher Voucher { get; set; }
        public Guid UserID { get; set; }
        public User User { get; set; }
        public Guid OrderID { get; set; }
        public Orders Order { get; set; }
        public DateTime RedeemedAt { get; set; }
        public decimal AmountDiscount { get; set; }
        
    }
}
