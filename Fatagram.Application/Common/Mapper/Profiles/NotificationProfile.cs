using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Domain.Models;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class NotificationProfile : Profile
    {
        public NotificationProfile()
        {
            CreateMap<Notification, NotificationDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId.ToString()))
                .ForMember(dest => dest.TimeDistance, opt => opt.MapFrom(src => src.CreatedAt.ToTimeDistance(DateTime.UtcNow)));

            CreateMap<NotificationDto, Notification>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => Guid.Parse(src.UserId)))
                .ForMember(dest => dest.Data, opt => opt.MapFrom(src => src.Data ?? new Dictionary<string, string>()));
        }
    }
}