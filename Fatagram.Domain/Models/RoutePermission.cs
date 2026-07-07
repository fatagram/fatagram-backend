namespace Fatagram.Domain.Models
{
    public class RoutePermission : BaseEntity
    {
        /// <summary>GET, POST, PUT, DELETE, PATCH, or * for any method.</summary>
        public string HttpMethod { get; set; } = "*";

        /// <summary>e.g. "api/v1/conversations/{conversationId}/messages" or "api/v1/conversations/{conversationId}/*"</summary>
        public string RoutePattern { get; set; } = string.Empty;

        /// <summary>Permission name required for this route, e.g. "conversation.member".</summary>
        public string PermissionName { get; set; } = string.Empty;

        /// <summary>Route parameter name whose value is the resource ID for scoped permission checks.</summary>
        public string? ResourceParam { get; set; }
        public bool IsActive { get; set; } = true;

        /// <summary>Lower order = higher priority when multiple patterns match.</summary>
        public int Order { get; set; } = 0;
    }
}
