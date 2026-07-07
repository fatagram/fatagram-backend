using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fatagram.Application.Abstractions.Security;
using Fatagram.Application.Services.JwtServices;
using Fatagram.Application.Types;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Fatagram.Application.Services.JwtServices
{
    public class JwtHmacSha256Service(IOptions<JwtSettings> jwtOptions)
        : IJwtService,
            IJwtTokenValidator
    {
        private readonly JwtSettings _settings = jwtOptions.Value;

        public Result<string> GenerateToken(Guid userId)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(_settings.ExpireInMinutes),
                signingCredentials: creds
            );

            return Result<string>.Create(
                ResponseStatusCode.Success,
                new JwtSecurityTokenHandler().WriteToken(token)
            );
        }

        public Result<string> GenerateToken(string userId) => GenerateToken(userId.ToGuid());

        public ClaimsPrincipal? ValidateToken(string token)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_settings.SecretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = _settings.Issuer,
                        ValidAudience = _settings.Audience,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out _
                );
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
