using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Fatagram.Application.Dtos.User.Update;
using Fatagram.Domain.Models;

namespace Fatagram.Application.Common.Mapper.Profiles
{
    public class UserPrivacyProfile : Profile
    {
        public UserPrivacyProfile()
        {
            CreateMap<UpdateUserPrivacyDto, UserPrivacy>();
            CreateMap<UserPrivacy, UpdateUserPrivacyDto>();
        }
    }
}