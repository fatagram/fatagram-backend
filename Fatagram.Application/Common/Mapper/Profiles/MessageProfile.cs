using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Application.Dtos.Message;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class MessageProfile : Profile
    {
        public MessageProfile()
        {
            CreateMap<Message, ResponseMessageDto>()
                .ForMember(
                    dest => dest.SenderAvatarUrl,
                    opt => opt.MapFrom(src => src.Sender.Avatar)
                );
            CreateMap<LastMessageProjection, ResponseMessageDto>();
            CreateMap<MessageMedia, MessageMediaDto>();
            CreateMap<MessageMediaDto, MessageMedia>();
        }
    }
}
