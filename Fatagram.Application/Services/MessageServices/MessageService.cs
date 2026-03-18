using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Application.Services.MessageServices.Interfaces;
using Fatagram.Infrastructure.Repositories.MessageRepository.Interfaces;

namespace Fatagram.Application.Services.MessageServices
{
    public class MessageService(IMessageRepository messageRepository) : IMessageService
    {
        private readonly IMessageRepository _messageRepository = messageRepository;

        public Task SendMessageAsync(Guid conversationId, Guid senderId, string content)
        {
            throw new NotImplementedException();
        }
    }
}
