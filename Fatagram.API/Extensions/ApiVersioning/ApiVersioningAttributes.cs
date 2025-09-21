namespace Fatagram.API.Extensions.ApiVersioning
{
    /// <summary>
    /// Attribute to specify API versions for controllers
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class ApiVersionAttribute : Attribute
    {
        public List<string> Versions { get; }

        public ApiVersionAttribute(params string[] versions)
        {
            Versions = versions?.ToList() ?? new List<string> { "v1" };
        }
    }

    /// <summary>
    /// Attribute to specify deprecated API versions
    /// </summary>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ApiVersionDeprecatedAttribute : Attribute
    {
        public string Version { get; }
        public string? DeprecatedSince { get; }
        public string? RemovalDate { get; }
        public string? ReplacementVersion { get; }

        public ApiVersionDeprecatedAttribute(string version, string? deprecatedSince = null, string? removalDate = null, string? replacementVersion = null)
        {
            Version = version;
            DeprecatedSince = deprecatedSince;
            RemovalDate = removalDate;
            ReplacementVersion = replacementVersion;
        }
    }

    /// <summary>
    /// Interface for API version provider
    /// </summary>
    public interface IApiVersionProvider
    {
        string GetRequestedVersion(HttpRequest request);
        string GetDefaultVersion();
        bool IsVersionSupported(string version);
        IEnumerable<string> GetSupportedVersions();
        bool IsVersionDeprecated(string version);
        ApiVersionDeprecatedAttribute? GetDeprecationInfo(string version);
    }

    /// <summary>
    /// Implementation of API version provider
    /// </summary>
    public class ApiVersionProvider : IApiVersionProvider
    {
        private readonly Dictionary<string, ApiVersionDeprecatedAttribute?> _supportedVersions;
        private const string DefaultVersion = "v1";

        public ApiVersionProvider()
        {
            _supportedVersions = new Dictionary<string, ApiVersionDeprecatedAttribute?>
            {
                { "v1", null }, // Not deprecated
                { "v2", null }  // Not deprecated
                // Add deprecated versions as needed:
                // { "v0.9", new ApiVersionDeprecatedAttribute("v0.9", "2024-01-01", "2024-12-31", "v1") }
            };
        }

        public string GetRequestedVersion(HttpRequest request)
        {
            // Priority order: URL segment > Header > Query parameter > Default
            
            // 1. Check URL path (/api/v1/users)
            var path = request.Path.Value;
            if (!string.IsNullOrEmpty(path))
            {
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var versionSegment = segments.FirstOrDefault(s => s.StartsWith("v") && s.Length > 1 && char.IsDigit(s[1]));
                if (!string.IsNullOrEmpty(versionSegment) && IsVersionSupported(versionSegment))
                {
                    return versionSegment;
                }
            }

            // 2. Check headers (X-Version: v1)
            if (request.Headers.TryGetValue("X-Version", out var headerVersion))
            {
                var version = headerVersion.FirstOrDefault();
                if (!string.IsNullOrEmpty(version) && IsVersionSupported(version))
                {
                    return version;
                }
            }

            // 3. Check query parameter (?version=v1)
            if (request.Query.TryGetValue("version", out var queryVersion))
            {
                var version = queryVersion.FirstOrDefault();
                if (!string.IsNullOrEmpty(version))
                {
                    // Add 'v' prefix if missing
                    if (!version.StartsWith("v"))
                        version = $"v{version}";
                        
                    if (IsVersionSupported(version))
                        return version;
                }
            }

            // 4. Check Accept header (Accept: application/json;v=1.0)
            var acceptHeader = request.Headers.Accept.FirstOrDefault();
            if (!string.IsNullOrEmpty(acceptHeader))
            {
                var versionMatch = System.Text.RegularExpressions.Regex.Match(acceptHeader, @"v=(\d+(?:\.\d+)?)");
                if (versionMatch.Success)
                {
                    var version = $"v{versionMatch.Groups[1].Value}";
                    if (IsVersionSupported(version))
                        return version;
                }
            }

            return GetDefaultVersion();
        }

        public string GetDefaultVersion() => DefaultVersion;

        public bool IsVersionSupported(string version) => _supportedVersions.ContainsKey(version);

        public IEnumerable<string> GetSupportedVersions() => _supportedVersions.Keys;

        public bool IsVersionDeprecated(string version) => 
            _supportedVersions.TryGetValue(version, out var deprecation) && deprecation != null;

        public ApiVersionDeprecatedAttribute? GetDeprecationInfo(string version) =>
            _supportedVersions.TryGetValue(version, out var deprecation) ? deprecation : null;
    }

    /// <summary>
    /// Middleware for handling API versioning
    /// </summary>
    public class ApiVersionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IApiVersionProvider _versionProvider;
        private readonly ILogger<ApiVersionMiddleware> _logger;

        public ApiVersionMiddleware(RequestDelegate next, IApiVersionProvider versionProvider, ILogger<ApiVersionMiddleware> logger)
        {
            _next = next;
            _versionProvider = versionProvider;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Skip versioning for non-API requests
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                await _next(context);
                return;
            }

            var requestedVersion = _versionProvider.GetRequestedVersion(context.Request);
            
            // Add version to request items for controllers to access
            context.Items["ApiVersion"] = requestedVersion;

            // Add version headers to response
            context.Response.Headers["X-API-Version"] = requestedVersion;
            context.Response.Headers["X-Supported-Versions"] = string.Join(", ", _versionProvider.GetSupportedVersions());

            // Check if version is deprecated
            if (_versionProvider.IsVersionDeprecated(requestedVersion))
            {
                var deprecationInfo = _versionProvider.GetDeprecationInfo(requestedVersion);
                context.Response.Headers["X-API-Deprecated"] = "true";
                
                if (deprecationInfo != null)
                {
                    if (!string.IsNullOrEmpty(deprecationInfo.DeprecatedSince))
                        context.Response.Headers["X-API-Deprecated-Since"] = deprecationInfo.DeprecatedSince;
                    
                    if (!string.IsNullOrEmpty(deprecationInfo.RemovalDate))
                        context.Response.Headers["X-API-Removal-Date"] = deprecationInfo.RemovalDate;
                    
                    if (!string.IsNullOrEmpty(deprecationInfo.ReplacementVersion))
                        context.Response.Headers["X-API-Replacement-Version"] = deprecationInfo.ReplacementVersion;
                }

                _logger.LogWarning("Deprecated API version {Version} used for path {Path}", requestedVersion, context.Request.Path);
            }

            // Validate version is supported
            if (!_versionProvider.IsVersionSupported(requestedVersion))
            {
                context.Response.StatusCode = 400;
                await context.Response.WriteAsync($"Unsupported API version: {requestedVersion}. Supported versions: {string.Join(", ", _versionProvider.GetSupportedVersions())}");
                return;
            }

            await _next(context);
        }
    }
}