using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.MessageServices
{
    public interface IMessageService
    {
        Task<Result<ResponseMessageDto>> SendMessageAsync(
            Guid? senderId,
            CreateMessageRequest request
        );

        Task<Result<CursorResult<ResponseMessageDto, int>>> GetMessagesAsync(
            Guid conversationId,
            CursorFilter<int> filter
        );

        Task<Result<List<ResponseMessageDto>>> GetDeltaMessagesAsync(
            Guid conversationId,
            int sinceSequenceNumber
        );
    }
}
