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
            // CreateMap<Conversation, ConversationDto>()
            //     .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
            //     .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
            //     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<Message, ResponseMessageDto>();
        }
    }
}
