using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class FriendshipExtension
    {
        public static void AddFriendship(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Friendship>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("friendships");
                entity
                    .HasOne(u => u.User1)
                    .WithMany(u => u.FriendshipAsUser1)
                    .HasForeignKey(u => u.User1Id)
                    .OnDelete(DeleteBehavior.Restrict);
                entity
                    .HasOne(u => u.User2)
                    .WithMany(u => u.FriendshipAsUser2)
                    .HasForeignKey(u => u.User2Id)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<FriendRequest>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("friend_requests");
                entity
                    .Property(e => e.ReceiverId)
                    .HasColumnName("receiver_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(e => e.SenderId)
                    .HasColumnName("sender_id")
                    .HasColumnType("uuid")
                    .IsRequired();
            });
        }
    }
}
