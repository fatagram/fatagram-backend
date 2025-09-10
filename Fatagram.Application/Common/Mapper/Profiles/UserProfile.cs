using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.User;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UpdateUserDto, User>()
                .ForAllMembers(opt => opt.Condition((src, dest, srcMember) =>
                    srcMember != null &&
                    (!(srcMember is DateTime) || !((DateTime)srcMember).Equals(default(DateTime))) &&
                    (!(srcMember is int) || !((int)srcMember).Equals(default(int))) &&
                    (!(srcMember is bool) || !((bool)srcMember).Equals(default(bool))) &&
                    (!(srcMember is Guid) || !((Guid)srcMember).Equals(default(Guid)))
                ));

            CreateMap<User, UserDto>();
        }
    }
}