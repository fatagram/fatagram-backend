using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.AuthServices
{
    public class GoogleOAuthService(IConfiguration config, ILogger<GoogleOAuthService> logger)
    {
        private readonly IConfiguration _config = config;
        private readonly ILogger<GoogleOAuthService> _logger = logger;

        public async Task<GoogleOAuthResponse> ExchangeCodeAsync(string code)
        {
            var client = new HttpClient();
            var dict = new Dictionary<string, string>
            {
                ["code"] = code,
                ["client_id"] =
                    _config["GoogleOAuth:ClientId"]
                    ?? throw new Exception("Google:ClientId not found in appsettings.json"),
                ["client_secret"] =
                    _config["GoogleOAuth:ClientSecret"]
                    ?? throw new Exception("Google:ClientSecret not found in appsettings.json"),
                ["redirect_uri"] =
                    _config["GoogleOAuth:RedirectUri"]
                    ?? throw new Exception("Google:RedirectUri not found in appsettings.json"),
                ["grant_type"] = "authorization_code",
            };
            var content = new FormUrlEncodedContent(dict);
            var response = await client.PostAsync(
                "https://accounts.google.com/o/oauth2/token",
                content
            );

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Google OAuth response: {json}", json);
            return JsonSerializer.Deserialize<GoogleOAuthResponse>(json)!;
        }

        public async Task<UserInfoResponse> GetUserInfoAsync(string accessToken)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            var response = await client.GetAsync("https://www.googleapis.com/oauth2/v3/userinfo");

            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            _logger.LogInformation("Google User Info response: {json}", json);
            return JsonSerializer.Deserialize<UserInfoResponse>(json)!;
        }
    }
}
