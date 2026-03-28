using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Utils;

namespace Fatagram.Application.Services.MessageServices.Interfaces
{
    public interface IMessageService
    {
        Task<Result<ResponseMessageDto>> SendMessageAsync(
            Guid? senderId,
            CreateMessageRequest request
        );

        Task<Result<CursorResult<ResponseMessageDto, DateTime>>> GetMessagesAsync(
            Guid conversationId,
            Guid userId,
            CursorFilter<DateTime> filter
        );
    }
}
