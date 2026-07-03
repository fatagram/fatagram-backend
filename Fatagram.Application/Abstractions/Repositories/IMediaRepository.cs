using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Application.Common.Projections;

namespace Fatagram.Application.Abstractions.Repositories
{
    public interface IMediaRepository : IBaseRepository<MessageMedia>
    {
        Task<List<MessageMedia>> GetMediaAroundAsync(
            Guid conversationId,
            Guid mediaId,
            bool before,
            int count
        );

        Task<MessageMediaAroundAnchorProjection> GetMediaAroundAnchorAsync(
            Guid conversationId,
            Guid? anchorMediaId,
            int count
        );
    }
}
