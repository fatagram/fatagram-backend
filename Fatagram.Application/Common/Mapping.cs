using AutoMapper;
using Fatagram.Application.Dtos.Account;
using Fatagram.Application.Dtos.User;
using Fatagram.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application.Common
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
                    srcMember != null &&
                    (!(srcMember is DateTime) || !((DateTime)srcMember).Equals(default(DateTime))) &&
                    (!(srcMember is int) || !((int)srcMember).Equals(default(int))) &&
                    (!(srcMember is bool) || !((bool)srcMember).Equals(default(bool))) &&
                    (!(srcMember is Guid) || !((Guid)srcMember).Equals(default(Guid))) &&
                    (!(srcMember is string) || !string.IsNullOrEmpty((string)srcMember))
                ));

            CreateMap<User, UserDto>();

            CreateMap<RegisterDto, Account>();
            CreateMap<RegisterDto, User>()
                .ForMember(dest => dest.UrlName, opt => opt.Ignore());


            CreateMap<UpdateUserPrivacyDto, UserPrivacy>();
            CreateMap<UserPrivacy, UpdateUserPrivacyDto>();
        }
    }
}
