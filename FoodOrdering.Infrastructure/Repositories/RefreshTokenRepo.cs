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
    public class RefreshTokenRepo : GenericRepo<RefreshTokens>, IRefreshTokenRepo
    {
        public RefreshTokenRepo(FoodOrderingDbContext context) : base(context) { }
    }
}
