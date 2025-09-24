namespace Food_Ordering.Models
{
    public class OrderItems
    {
        public Guid Id { get; set; }
        public Orders Orders { get; set; }
        public Guid OrderId { get; set; }
        public MenuItems MenuItems { get; set; }
        public Guid MenuItemsId { get; set; }
        public int Quantity { get; set; }
        public int UnitPrice { get; set; }
        public int SubTotal { get; set; }
    }
}
