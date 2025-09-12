using Food_Ordering.DTOs.Response;
using Food_Ordering.Models;

namespace Food_Ordering.Services.Auth
{
    public interface IJwtService
    {
        public Task<Token> GenerateToken(User user);
    }
}
