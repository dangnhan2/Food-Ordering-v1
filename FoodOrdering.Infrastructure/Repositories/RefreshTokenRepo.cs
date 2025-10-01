using FoodOrdering.Application.Extension;
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
    public class RefreshTokenRepo : GenericRepo<RefreshTokens>, IRefreshTokenRepo
    {
        private readonly FoodOrderingDbContext _context;
        public RefreshTokenRepo(FoodOrderingDbContext context) : base(context) {
           _context = context;
        }

        public async Task<RefreshTokens?> GetTokenByRefreshToken(string refreshToken)
        {
            return await _context.RefreshTokens
                .Include(t => t.User)
                .FirstOrDefaultAsync(t => t.Token == refreshToken.HashToken());
        }
    }
}
