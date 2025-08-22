using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class UserExtension
    {
        public static void AddUser(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .Property(u => u.Version)
                .IsRowVersion();

            modelBuilder.Entity<User>()
                .HasOne(u => u.Language)
                .WithMany(l => l.Users)
                .HasForeignKey(u => u.LanguageCode)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
