using Fatagram.Application.Services.AuthServices.OAuth.Google;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Fatagram.Application.Services.AuthServices.OAuth
{
    /// <summary>
    /// OAuth provider types
    /// </summary>
    public enum OAuthProvider
    {
        Google,
        Facebook,
        GitHub,
        Microsoft,
    }

    /// <summary>
    /// Factory for creating OAuth service instances
    /// </summary>
    public class OAuthServiceFactory
    {
        private readonly IConfiguration _configuration;
        private readonly ILoggerFactory _loggerFactory;

        public OAuthServiceFactory(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _loggerFactory = loggerFactory;
        }

        /// <summary>
        /// Create OAuth service based on provider type
        /// </summary>
        /// <param name="provider">OAuth provider type</param>
        /// <returns>OAuth service instance</returns>
        /// <exception cref="NotSupportedException">When provider is not supported</exception>
        public IOAuthService CreateService(OAuthProvider provider)
        {
            return provider switch
            {
                OAuthProvider.Google => CreateGoogleService(),
                // OAuthProvider.Facebook => CreateFacebookService(),
                // OAuthProvider.GitHub => CreateGitHubService(),
                // OAuthProvider.Microsoft => CreateMicrosoftService(),
                _ => throw new NotSupportedException($"OAuth provider {provider} is not supported"),
            };
        }

        private IOAuthService CreateGoogleService()
        {
            return new GoogleOAuthService(
                _configuration,
                _loggerFactory.CreateLogger<GoogleOAuthService>()
            );
        }

        // Example for future providers:
        // private IOAuthService CreateFacebookService()
        // {
        //     return new FacebookOAuthService(_configuration, _loggerFactory.CreateLogger<FacebookOAuthService>());
        // }
    }
}
