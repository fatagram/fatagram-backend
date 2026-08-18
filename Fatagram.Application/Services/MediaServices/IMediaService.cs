using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.MediaServices
{
    public interface IMediaService
    {
        Task<Result<List<MessageMediaDto>>> GetMediaAroundAsync(
            Guid conversationId,
            Guid mediaId,
            bool before,
            int count
        );

        Task<Result<CursorResult<MessageMediaDto, int>>> GetConversationMediaAsync(
            Guid conversationId,
            List<Fatagram.Domain.Enums.MediaType>? types,
            Fatagram.Application.Dtos.Filter.CursorFilter<int> filter
        );

        Task<Result<MediaAroundAnchorDto>> GetMediaAroundAnchorAsync(
            Guid conversationId,
            Guid? anchorMediaId,
            int count
        );
    }
}
