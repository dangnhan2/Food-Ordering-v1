using Food_Ordering.Data;
using Food_Ordering.Models;
using Microsoft.EntityFrameworkCore;

namespace Food_Ordering.Repositories
{
    public class RefreshTokenRepo : IRefreshTokenRepo
    {   
        private readonly ApplicationDbContext _applicationDbContext;

        public RefreshTokenRepo(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }
        public async Task AddTokenAsync(RefreshTokens refreshTokens)
        {
            await _applicationDbContext.RefreshTokens.AddAsync(refreshTokens);
        }

        public async Task<RefreshTokens?> GetRefreshTokenAsync(string refreshToken)
        {
            return await _applicationDbContext.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == refreshToken);
        }

        public void Remove(RefreshTokens refreshTokens)
        {
            _applicationDbContext.RefreshTokens.Remove(refreshTokens);
        }
    }
}
