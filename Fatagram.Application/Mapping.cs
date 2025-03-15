using AutoMapper;
using Fatagram.Application.Dtos;
using Fatagram.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Application
{
    public class Mapping : Profile
    {
        public Mapping()
        {
            CreateMap<UpdateUserDto, User>();
            CreateMap<RegisterDto, Account>();
            CreateMap<RegisterDto, User>();
            CreateMap<User, UserDto>();
        }
    }
}
