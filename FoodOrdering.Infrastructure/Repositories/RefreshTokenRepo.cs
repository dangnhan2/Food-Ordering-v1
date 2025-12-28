using FoodOrdering.Application.Extension;
using FoodOrdering.Application.Helper.Extensions;
using FoodOrdering.Application.Repositories;
using FoodOrdering.Domain.Models;
using FoodOrdering.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FoodOrdering.Infrastructure.Repository
{
    public class RefreshTokenRepo : GenericRepo<RefreshToken>, IRefreshTokenRepo
    {
        private readonly FoodOrderingDbContext _context;
        public RefreshTokenRepo(FoodOrderingDbContext context) : base(context) {
           _context = context;
        }

        public async Task<RefreshToken?> GetTokenByRefreshToken(string refreshToken)
        {
            return await _context.RefreshToken
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == refreshToken.HashToken());
        }
    }
}
