using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Dtos.Message;

namespace Fatagram.Application.Services.MessageServices.Interfaces
{
    public interface IMessageSender
    {
        Task SendMessageAsync(Guid conversationId, ResponseMessageDto message);
        Task SendMessageAllAsync(ResponseMessageDto message, IEnumerable<Guid> conversationIds);
    }
}
