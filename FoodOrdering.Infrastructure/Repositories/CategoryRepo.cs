using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repository
{
    public class CategoryRepo : GenericRepo<Categories>, ICategoryRepo
    {
        public CategoryRepo(FoodOrderingDbContext context) : base(context) { }
    }
}
