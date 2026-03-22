using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Fatagram.Application.Utils;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Repositories.ConversationRepository.Interfaces;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.ConversationServices
{
    public class ConversationService(IConversationRepository conversationRepository, IMapper mapper)
        : IConversationService
    {
        private readonly IConversationRepository _conversationRepository = conversationRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<CursorResult<ConversationDto, DateTime>>> GetAllAsync(
            Guid userId,
            CursorFilter<DateTime>? cursor = null
        )
        {
            var conservations = await _conversationRepository.GetMyConversationsAsync(
                userId.ToString(),
                cursor?.Cursor,
                cursor?.Limit ?? 20
            );
            var res = _mapper.Map<List<ConversationDto>>(conservations);
            Console.WriteLine(
                $"[ConversationService] GetAllAsync: {res.Count} conversations retrieved for user {userId}"
            );
            return Result<CursorResult<ConversationDto, DateTime>>.Create(
                ResponseStatusCode.Success,
                new CursorResult<ConversationDto, DateTime>
                {
                    Items = res,
                    NextCursor = conservations.Count > 0 ? conservations.Last().LastActiveAt : null,
                    HasNext = conservations.Count == (cursor?.Limit ?? 20),
                }
            );
        }

        public async Task<Result<ConversationDto>> GetByIdAsync(Guid userId, Guid conversationId)
        {
            var conversation = await _conversationRepository.GetConversationById(
                userId,
                conversationId
            );
            return Result<ConversationDto>.Create(
                ResponseStatusCode.Success,
                _mapper.Map<ConversationDto>(conversation)
            );
        }

        public async Task<Result<ConversationDto>> GetWithAsync(Guid userId, Guid targetUserId)
        {
            var conversation = await _conversationRepository.GetConversationWith(
                userId,
                targetUserId
            );

            return Result<ConversationDto>.Create(
                ResponseStatusCode.Success,
                _mapper.Map<ConversationDto>(conversation)
            );
        }

        public Task<Result<ConversationDto>> CreateAsync(Guid creatorId, string conversationType)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ConversationDto>> CreateAsync(Guid creatorId, Guid otherUserId)
        {
            throw new NotImplementedException();
        }

        public Task<Result<ConversationDto>> CreateConversationAsync(
            Guid creatorId,
            string conversationType
        )
        {
            throw new NotImplementedException();
        }

        public Task<Result<ConversationDto>> CreateGroupAsync(
            Guid creatorId,
            IEnumerable<Guid> participantIds,
            string groupName
        )
        {
            throw new NotImplementedException();
        }
    }
}
