using DotNetEnv;
using Food_Ordering.DTOs.Response;
using Food_Ordering.Models;
using Food_Ordering.Repositories.UnitOfWork;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Food_Ordering.Services.Auth
{
    public class JwtService : IJwtService
    {
        private readonly UserManager<User> _userManager;
        private readonly SymmetricSecurityKey _symmetricSecurityKey;
        private readonly IUnitOfWork _unitOfWork;
        public JwtService(UserManager<User> userManager, IUnitOfWork unitOfWork)
        {
            Env.Load();
            _userManager = userManager;
            _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Env.GetString("KEY")));
            _unitOfWork = unitOfWork;
        }

        public async Task<Token> GenerateToken(User user)
        {
            Env.Load();

            var credential = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var issuer = Env.GetString("ISSUER");
            var audience = Env.GetString("AUDIENCE");

            var claims = new List<Claim>
            {
                 new Claim(ClaimTypes.NameIdentifier, user.Id),
                 new Claim(ClaimTypes.Name, user.Email),
                 new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var roles = await _userManager.GetRolesAsync(user);

            claims.AddRange(roles.Select(r => new Claim(ClaimTypes.Role, r)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {   
                Subject = new ClaimsIdentity(claims),
                SigningCredentials = credential,
                Issuer = issuer,
                Audience = audience,
                Expires = DateTime.UtcNow.AddMinutes(15)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var jwt = tokenHandler.CreateToken(tokenDescriptor);
            var token = tokenHandler.WriteToken(jwt);

            var refreshToken = new RefreshTokens
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString(),
                ExpriedAt = DateTime.UtcNow.AddDays(7),
            };

            await _unitOfWork.RefreshToken.AddTokenAsync(refreshToken);
            await _unitOfWork.SaveAsync();

            return new Token
            {
                AccessToken = token,
                RefreshToken = refreshToken.Token
            };
        }
    }
}
