using Fatagram.Domain.Models.UserInformations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Data.Extensions.UserInformations
{
    public static class SkillExtension
    {
        public static void AddSkill(this ModelBuilder modelBuilder)
        { 
            // Add seed data

            modelBuilder.Entity<Skill>().HasData(
                new Skill() { Id = new Guid("55555555-5555-5555-5555-555555555555"), LocalizationKey = "SKILL_PROGRAMMING", CreatedAt = new DateTime(), UpdatedAt = new DateTime() },
                new Skill() { Id = new Guid("66666666-6666-6666-6666-666666666666"), LocalizationKey = "SKILL_DESIGN", CreatedAt = new DateTime(), UpdatedAt = new DateTime() },
                new Skill() { Id = new Guid("77777777-7777-7777-7777-777777777777"), LocalizationKey = "SKILL_MANAGEMENT", CreatedAt = new DateTime(), UpdatedAt = new DateTime() },
                new Skill() { Id = new Guid("88888888-8888-8888-8888-888888888888"), LocalizationKey = "SKILL_MARKETING", CreatedAt = new DateTime(), UpdatedAt = new DateTime() }
            );
        }
    }
}
