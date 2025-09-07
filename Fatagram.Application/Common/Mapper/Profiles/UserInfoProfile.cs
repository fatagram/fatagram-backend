using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Domain.Models;
using Fatagram.Infrastructure.Projections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class UserInfoProfile : Profile
    {
        public UserInfoProfile()
        {
            CreateMap<UserInfoV2, UserInfoOverview>()
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

            CreateMap<User, UserInfoOverview>()
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
                .ForMember(dest => dest.Bio, opt => opt.MapFrom(src => src.Bio));
        }
    }
}
