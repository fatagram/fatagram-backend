using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.Application.Services.MessageServices.Interfaces
{
    public interface IMessageService
    {
        Task SendMessageAsync(Guid conversationId, Guid senderId, string content);
    }
}
