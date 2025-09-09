namespace Food_Ordering.Models
{
    public class MenuCategories
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string? Description { get; set; }
        public ICollection<MenuItems> MenuItems { get; set; } = new List<MenuItems>();
    }
}
