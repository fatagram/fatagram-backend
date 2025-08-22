using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<NotificationProjection, NotificationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.ActorId, opt => opt.MapFrom(src => src.ActorId.ToString()))
                .ForMember(dest => dest.TimeDistance, opt => opt.MapFrom(src => src.CreatedAt.ToTimeDistance(DateTime.UtcNow)));

            CreateMap<NotificationDto, Notification>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToGuid()))
                .ForMember(dest => dest.ActorId, opt => opt.MapFrom(src => src.ActorId.ToGuid()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.TimeDistance.ToDateTime()));
        }
    }
}