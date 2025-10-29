using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class UserPrivacyExtension
    {
        public static void AddUserPrivacy(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<UserPrivacy>()
                .HasOne(up => up.User)
                .WithMany(u => u.Privacies)
                .HasForeignKey(up => up.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
