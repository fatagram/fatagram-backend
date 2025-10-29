using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class AccountExtension
    {
        public static void AddAccount(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<Account>()
                .HasOne(a => a.User)
                .WithMany(u => u.Accounts)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>().Property(a => a.Version).IsRowVersion();
        }
    }
}
