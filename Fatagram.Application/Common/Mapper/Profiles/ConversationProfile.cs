using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Application.Dtos.Conversation;
using Fatagram.Domain.Models;
using Fatagram.Application.Common.Projections;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class ConversationProfile : Profile
    {
        public ConversationProfile()
        {
            // CreateMap<Conversation, ConversationDto>()
            //     .ForMember(dest => dest.Participants, opt => opt.MapFrom(src => src.Participants))
            //     .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
            //     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<ConversationProjection, ConversationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(
                    dest => dest.OtherUserId,
                    opt => opt.MapFrom(src => src.OtherUserId.ToString())
                )
                .ForMember(dest => dest.LastMessage, opt => opt.MapFrom(src => src.LastMessage));

            CreateMap<Conversation, ConversationProjection>()
                .ForMember(
                    dest => dest.ParticipantIds,
                    opt => opt.MapFrom(src => src.Participants.Select(p => p.UserId).ToList())
                );

            CreateMap<UpdateConversationDto, Conversation>()
                .ForAllMembers(opt =>
                    opt.Condition(
                        (src, dest, srcMember) =>
                            srcMember != null
                            && (
                                !(srcMember is DateTime)
                                || !((DateTime)srcMember).Equals(default(DateTime))
                            )
                            && (!(srcMember is int) || !((int)srcMember).Equals(default(int)))
                            && (!(srcMember is bool) || !((bool)srcMember).Equals(default(bool)))
                            && (!(srcMember is Guid) || !((Guid)srcMember).Equals(default(Guid)))
                    )
                );
        }
    }
}
