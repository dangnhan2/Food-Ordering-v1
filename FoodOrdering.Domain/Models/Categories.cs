using Food_Ordering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Domain.Models
{
    public class Categories
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public ICollection<Menus> Menus { get; set; } = new List<Menus>();
    }
}
