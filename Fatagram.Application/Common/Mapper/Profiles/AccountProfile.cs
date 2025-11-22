using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.Auth;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class AccountProfile : Profile
    {
        public AccountProfile()
        {
            // CreateMap<Account, AccountDto>()
            //     .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
            //     .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

            CreateMap<RegisterDto, Account>();
            CreateMap<RegisterDto, User>().ForMember(dest => dest.UrlName, opt => opt.Ignore());
        }
    }
}
