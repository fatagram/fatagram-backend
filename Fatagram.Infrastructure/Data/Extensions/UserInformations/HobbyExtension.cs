using Fatagram.Domain.Models.UserInformations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Data.Extensions.UserInformations
{
    public static class HobbyExtension
    {
        public static void AddHobby(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Hobby>().HasData(
                new Hobby() { Id = new Guid("11111111-1111-1111-1111-111111111111"), LocalizationKey = "HOBBY_MUSIC", CreatedAt = new DateTime(), UpdatedAt = new DateTime() },
                new Hobby() { Id = new Guid("22222222-2222-2222-2222-222222222222"), LocalizationKey = "HOBBY_SPORT", CreatedAt = new DateTime(), UpdatedAt = new DateTime() },
                new Hobby() { Id = new Guid("33333333-3333-3333-3333-333333333333"), LocalizationKey = "HOBBY_TRAVEL", CreatedAt = new DateTime(), UpdatedAt = new DateTime() },
                new Hobby() { Id = new Guid("44444444-4444-4444-4444-444444444444"), LocalizationKey = "HOBBY_READING", CreatedAt = new DateTime(), UpdatedAt = new DateTime() }
            );
        }
    }
}
