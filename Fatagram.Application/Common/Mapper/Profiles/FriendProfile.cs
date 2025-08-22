using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using Fatagram.Shared.Extensions;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class FriendProfile : Profile
    {
        public FriendProfile()
        {
            CreateMap<FriendRequest, FriendRequestDto>()
                .ForMember(dest => dest.SenderAvatar, opt => opt.MapFrom(src => src.Sender.Avatar))
                .ForMember(dest => dest.SenderUrlName, opt => opt.MapFrom(src => src.Sender.UrlName))
                .ForMember(dest => dest.SenderName, opt => opt.MapFrom(src => src.Sender.FullName))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt.ToTimeDistance(DateTime.UtcNow)));

            CreateMap<FriendProjection, FriendDto>()
                .ConstructUsing(src => new FriendDto
                {
                    Id = src.User.Id,
                    Avatar = src.User.Avatar,
                    Name = src.User.FullName,
                    UrlName = src.User.UrlName,
                    IsFriend = src.IsFriend
                });
        }
    }
}