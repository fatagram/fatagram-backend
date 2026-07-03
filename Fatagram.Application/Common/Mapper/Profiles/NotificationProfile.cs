using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Notification;
using Fatagram.Domain.Models;
using Fatagram.Application.Common.Projections;
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
                .ForMember(dest => dest.ActorId, opt => opt.MapFrom(src => src.ActorId.ToString()));

            CreateMap<NotificationDto, Notification>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToGuid()))
                .ForMember(dest => dest.ActorId, opt => opt.MapFrom(src => src.ActorId.ToGuid()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type))
                .ForMember(dest => dest.Data, opt => opt.Ignore())
                .ForMember(dest => dest.SourceId, opt => opt.MapFrom(src => src.SourceId.ToGuid()))
                .AfterMap(
                    (src, dest) =>
                    {
                        dest.Data = src.Data != null ? JsonSerializer.Serialize(src.Data) : null;
                    }
                );

            CreateMap<UserNotification, NotificationDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
                .ForMember(
                    dest => dest.ActorId,
                    opt => opt.MapFrom(src => src.Notification.ActorId.ToString())
                )
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Notification.Type))
                .ForMember(
                    dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => src.Notification.CreatedAt)
                )
                .ForMember(
                    dest => dest.SourceId,
                    opt => opt.MapFrom(src => src.Notification.SourceId.ToString())
                )
                .ForMember(dest => dest.Data, opt => opt.Ignore())
                .AfterMap(
                    (src, dest) =>
                    {
                        dest.Data = string.IsNullOrEmpty(src.Notification.Data)
                            ? new Dictionary<string, string>()
                            : JsonSerializer.Deserialize<Dictionary<string, string>>(
                                src.Notification.Data
                            ) ?? new Dictionary<string, string>();
                    }
                );
        }
    }
}
