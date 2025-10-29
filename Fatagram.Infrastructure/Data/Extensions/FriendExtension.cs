using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class FriendExtension
    {
        public static void AddFriend(this ModelBuilder modelBuilder)
        {
            modelBuilder
                .Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany(u => u.FriendRequests)
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany(u => u.FriendRequestsReceived)
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Friendship>()
                .HasOne(f => f.User1)
                .WithMany(f => f.FriendshipAsUser1)
                .HasForeignKey(f => f.User1Id)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder
                .Entity<Friendship>()
                .HasOne(f => f.User2)
                .WithMany(f => f.FriendshipAsUser2)
                .HasForeignKey(f => f.User2Id)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
