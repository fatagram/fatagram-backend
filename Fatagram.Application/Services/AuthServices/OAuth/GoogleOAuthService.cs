using System.Text.Json;
using Fatagram.Application.Dtos.Auth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.AuthServices.OAuth.Google
{
    /// <summary>
    /// Google OAuth service - handles OAuth flow and user info retrieval
    /// </summary>
    public class GoogleOAuthService : IOAuthService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<GoogleOAuthService> _logger;

        public GoogleOAuthService(IConfiguration config, ILogger<GoogleOAuthService> logger)
        {
            _config = config;
            _logger = logger;
        }

        /// <summary>
        /// Get user information from Google OAuth using authorization code
        /// </summary>
        public async Task<OAuthUserInfo> GetUserInfoAsync(
            string code,
            string? redirectUriOverride = null
        )
        {
            var oauthResponse = await ExchangeCodeAsync(code, redirectUriOverride);
            _logger.LogInformation(
                "Google OAuth token received, expires in: {ExpiresIn}s",
                oauthResponse.ExpiresIn
            );

            var googleUserInfo = await GetGoogleUserInfoAsync(oauthResponse.AccessToken);
            _logger.LogInformation(
                "Google user info: {Email}, {Name}, {Picture}",
                googleUserInfo.Email,
                googleUserInfo.Name,
                googleUserInfo.Picture
            );

            // Step 3: Map to common OAuthUserInfo
            return new OAuthUserInfo
            {
                Email = googleUserInfo.Email,
                Name = googleUserInfo.Name,
                GivenName = googleUserInfo.GivenName,
                FamilyName = googleUserInfo.FamilyName,
                Picture = googleUserInfo.Picture,
            };
        }

        private async Task<GoogleOAuthResponse> ExchangeCodeAsync(
            string code,
            string? redirectUriOverride = null
        )
        {
            var defaultUri =
                _config["GoogleOAuth:RedirectUri"]
                ?? throw new Exception("Google:RedirectUri not found in appsettings.json");
            var adminUri = _config["GoogleOAuth:AdminRedirectUri"];

            if (redirectUriOverride is not null)
            {
                var allowed = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { defaultUri };
                if (adminUri is not null)
                    allowed.Add(adminUri);

                if (!allowed.Contains(redirectUriOverride))
                    throw new Exception($"Redirect URI not allowed: {redirectUriOverride}");
            }

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
                ["redirect_uri"] = redirectUriOverride ?? defaultUri,
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

        private async Task<UserInfoResponse> GetGoogleUserInfoAsync(string accessToken)
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
