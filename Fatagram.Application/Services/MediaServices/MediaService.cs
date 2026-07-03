using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Message;
using Fatagram.Application.Services.MediaServices;
using Fatagram.Application.Utils;
using Fatagram.Application.Abstractions.Repositories;
using Fatagram.Shared.Enums;

namespace Fatagram.Application.Services.MediaServices
{
    public class MediaService(IMediaRepository mediaRepository, IMapper mapper) : IMediaService
    {
        private readonly IMediaRepository _mediaRepository = mediaRepository;
        private readonly IMapper _mapper = mapper;

        public async Task<Result<MediaAroundAnchorDto>> GetMediaAroundAnchorAsync(
            Guid conversationId,
            Guid? anchorMediaId,
            int count
        )
        {
            var media = await _mediaRepository.GetMediaAroundAnchorAsync(
                conversationId,
                anchorMediaId,
                count
            );

            media.Left.Reverse();

            return Result<MediaAroundAnchorDto>.Create(
                ResponseStatusCode.Success,
                new MediaAroundAnchorDto
                {
                    Left = _mapper.Map<List<MessageMediaDto>>(media.Left),
                    Right = _mapper.Map<List<MessageMediaDto>>(media.Right),
                }
            );
        }

        public async Task<Result<List<MessageMediaDto>>> GetMediaAroundAsync(
            Guid conversationId,
            Guid mediaId,
            bool before,
            int count
        )
        {
            var media = await _mediaRepository.GetMediaAroundAsync(
                conversationId,
                mediaId,
                before,
                count
            );

            return Result<List<MessageMediaDto>>.Create(
                ResponseStatusCode.Success,
                _mapper.Map<List<MessageMediaDto>>(media)
            );
        }
    }
}
