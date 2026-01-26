using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using FoodOrdering.Infrastructure.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repositories
{
    public class AdvertisementRepo : GenericRepo<Advertisement>, IAdvertisementRepo
    {
        public AdvertisementRepo (FoodOrderingDbContext context) : base (context) { }
    }
}
