using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fatagram.Application.Exceptions.MiddleLevelExceptions;
using Fatagram.Application.Services.JwtServices;
using Fatagram.Application.Utils;
using Fatagram.Shared.Enums;
using Fatagram.Shared.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Fatagram.Application.Services.JwtServices
{
    /// <summary>
    /// Service for generating JWT tokens
    /// </summary>
    public class JwtHmacSha256Service(IConfiguration config) : IJwtService
    {
        private readonly IConfiguration _config = config;

        /// <summary>
        /// Generate a JWT token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<string> GenerateToken(Guid userId)
        {
            var secretKey = _config["JwtSettings:SecretKey"] ?? "";
            var issuer = _config["JwtSettings:Issuer"] ?? "";
            var audience = _config["JwtSettings:Audience"] ?? "";
            var expirationInMinutes = int.Parse(_config["JwtSettings:ExpireInMinutes"] ?? "60");

            // if (secretKey == null) throw new ArgumentNullException("Secret key is null");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
                signingCredentials: creds
            );

            return Result<string>.Create(
                ResponseStatusCode.Success,
                new JwtSecurityTokenHandler().WriteToken(token)
            );
        }

        /// <summary>
        /// Generate a JWT token
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<string> GenerateToken(string userId) => GenerateToken(userId.ToGuid());

        /// <summary>
        /// Validate a JWT token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<ClaimsPrincipal> ValidateToken(string token)
        {
            var secretKey = _config["JwtSettings:SecretKey"] ?? "";
            var issuer = _config["JwtSettings:Issuer"] ?? "";
            var audience = _config["JwtSettings:Audience"] ?? "";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(
                    token,
                    new TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = issuer,
                        ValidAudience = audience,
                        IssuerSigningKey = key,
                        ClockSkew = TimeSpan.Zero,
                    },
                    out var validatedToken
                );

                return Result<ClaimsPrincipal>.Create(ResponseStatusCode.Success, principal);
            }
            catch (Exception)
            {
                return Result<ClaimsPrincipal>.Create(ResponseStatusCode.Unauthorized);
            }
        }
    }
}
