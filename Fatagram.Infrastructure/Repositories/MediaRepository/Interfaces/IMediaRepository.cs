using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Fatagram.Infrastructure.Repositories.BaseRepository.Interfaces;

namespace Fatagram.Infrastructure.Repositories.MediaRepository.Interfaces
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
