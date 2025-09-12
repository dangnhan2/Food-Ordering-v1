using Food_Ordering.Models;

namespace Food_Ordering.Repositories
{
    public interface IRefreshTokenRepo
    {      
            public Task<RefreshTokens?> GetRefreshTokenAsync(string refreshToken);
            public Task AddTokenAsync(RefreshTokens refreshTokens);
            public void Remove(RefreshTokens refreshTokens);
    }
}
