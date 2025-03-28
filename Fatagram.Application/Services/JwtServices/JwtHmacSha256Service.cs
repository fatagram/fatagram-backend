using Fatagram.Application.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fatagram.Application.Services.JwtServices.Interface;
using Fatagram.Shared.Utils;

namespace Fatagram.Application.Services.JwtServices
{
    /// <summary>
    /// Service for generating JWT tokens
    /// </summary>
    public class JwtHmacSha256Service : IJwtService
    {

        private readonly IConfiguration _config;

        public JwtHmacSha256Service(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Generate a JWT token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<string> GenerateToken(string username, Guid userId)
        {
            var secretKey = _config["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey is missing");
            var issuer = _config["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer is missing"); ;
            var audience = _config["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience is missing");

            if (!int.TryParse(_config["JwtSettings:ExpireInMinutes"], out var expirationInMinutes))
            {
                expirationInMinutes = 60;
            }

            // if (secretKey == null) throw new ArgumentNullException("Secret key is null");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationInMinutes),
                signingCredentials: creds
            );

            return Result<string>.Success(new JwtSecurityTokenHandler().WriteToken(token));
        }


        /// <summary>
        /// Generate a JWT token
        /// </summary>
        /// <param name="username"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public Result<string> GenerateToken(string username, string userId) => GenerateToken(username, Guid.Parse(userId));


        /// <summary>
        /// Validate a JWT token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="errorCode"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public Result<ClaimsPrincipal> ValidateToken(string token)
        {
            var secretKey = _config["JwtSettings:SecretKey"] ?? throw new ArgumentNullException("JwtSettings:SecretKey is missing");
            var issuer = _config["JwtSettings:Issuer"] ?? throw new ArgumentNullException("JwtSettings:Issuer is missing"); ;
            var audience = _config["JwtSettings:Audience"] ?? throw new ArgumentNullException("JwtSettings:Audience is missing");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = issuer,
                    ValidAudience = audience,
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero
                }, out var validatedToken);

                return Result<ClaimsPrincipal>.Success(principal);
            }
            catch (Exception)
            {
                // If the token is invalid, return an empty ClaimsPrincipal
                return Result<ClaimsPrincipal>.Failure(ErrorCodes.ACCESS_TOKEN_INVALID);
            }
        }
    }
}
