using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class ConversationExtension
    {
        public static void AddConversation(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Conversation>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("conversations");
                entity
                    .Property(c => c.Name)
                    .HasColumnName("name")
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired(false);
                entity
                    .Property(c => c.IsGroup)
                    .HasColumnName("is_group")
                    .HasColumnType("boolean")
                    .HasDefaultValue(false)
                    .IsRequired();
                entity
                    .Property(c => c.UniqueConversationKey)
                    .HasColumnName("unique_conversation_key")
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired(false);
                entity.HasIndex(c => c.UniqueConversationKey).IsUnique();
            });

            modelBuilder.Entity<ConversationParticipant>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("conversation_participants");
                entity
                    .Property(cp => cp.ConversationId)
                    .HasColumnName("conversation_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(cp => cp.UserId)
                    .HasColumnName("user_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(cp => cp.LastSeenMessageId)
                    .HasColumnName("last_seen_message_id")
                    .HasColumnType("uuid");
                entity
                    .Property(cp => cp.Role)
                    .HasColumnName("role")
                    .HasColumnType("integer")
                    .HasConversion<int>()
                    .HasDefaultValue(ConversationRole.Member)
                    .IsRequired();
                entity
                    .Property(cp => cp.Nickname)
                    .HasColumnName("nickname")
                    .HasColumnType("VARCHAR(255)")
                    .IsRequired(false);
                entity
                    .HasOne(cp => cp.Conversation)
                    .WithMany(c => c.Participants)
                    .HasForeignKey(cp => cp.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(cp => cp.User)
                    .WithMany(u => u.ConversationParticipants)
                    .HasForeignKey(cp => cp.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(cp => cp.LastSeenMessage)
                    .WithMany()
                    .HasForeignKey(cp => cp.LastSeenMessageId)
                    .OnDelete(DeleteBehavior.SetNull);
                entity.HasIndex(cp => new { cp.ConversationId, cp.UserId }).IsUnique();
            });

            modelBuilder.Entity<Message>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("messages");
                entity
                    .Property(m => m.ConversationId)
                    .HasColumnName("conversation_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(m => m.SenderId)
                    .HasColumnName("sender_id")
                    .HasColumnType("uuid")
                    .IsRequired(false);
                entity
                    .Property(m => m.Content)
                    .HasColumnName("content")
                    .HasColumnType("text")
                    .IsRequired();
                entity
                    .Property(m => m.ReadAt)
                    .HasColumnName("read_at")
                    .HasColumnType("timestamptz")
                    .IsRequired(false);
                entity
                    .Property(m => m.Type)
                    .HasColumnName("type")
                    .HasColumnType("integer")
                    .HasConversion<int>()
                    .HasDefaultValue(MessageType.Text)
                    .IsRequired();
                entity
                    .Property(m => m.Metadata)
                    .HasColumnName("metadata")
                    .HasColumnType("jsonb")
                    .IsRequired(false);
                entity
                    .HasOne(m => m.Conversation)
                    .WithMany(c => c.Messages)
                    .HasForeignKey(m => m.ConversationId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity
                    .HasOne(m => m.Sender)
                    .WithMany(u => u.Messages)
                    .HasForeignKey(m => m.SenderId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(m => m.ConversationId);
            });
        }
    }
}
