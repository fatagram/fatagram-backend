using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Fatagram.Domain.Enums;
using Fatagram.Domain.Models;
using Microsoft.EntityFrameworkCore;
using NpgsqlTypes;

namespace Fatagram.Infrastructure.Data.Extensions
{
    public static class ConversationExtension
    {
        public static void AddConversation(this ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("unaccent");
            modelBuilder.HasPostgresExtension("pg_trgm");

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
                    .Property(c => c.BackgroundUrl)
                    .HasColumnName("background_url")
                    .HasColumnType("text")
                    .IsRequired(false);
                entity
                    .Property(c => c.Theme)
                    .HasColumnName("theme")
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
                entity
                    .Property(c => c.LastMessageNumber)
                    .HasColumnName("last_message_number")
                    .HasColumnType("integer")
                    .HasDefaultValue(0)
                    .IsRequired();

                entity
                    .Property(c => c.SearchText)
                    .HasColumnName("search_text")
                    .HasColumnType("text")
                    .IsRequired(false);

                entity.HasIndex(c => c.SearchText).HasMethod("GIN").HasOperators("gin_trgm_ops");

                entity.HasIndex(c => c.UniqueConversationKey).IsUnique();

                entity
                    .Property<NpgsqlTsVector>("SearchVector")
                    .HasColumnName("search_vector")
                    .HasComputedColumnSql(
                        "to_tsvector('simple', coalesce(search_text, ''))",
                        stored: true
                    );

                entity.HasIndex("SearchVector").HasMethod("GIN");
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
                    .Property(cp => cp.SeenAt)
                    .HasColumnName("seen_at")
                    .HasColumnType("timestamptz")
                    .IsRequired(false);
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
                    .Property(cp => cp.LastSeenNumber)
                    .HasColumnName("last_seen_number")
                    .HasColumnType("integer")
                    .HasDefaultValue(0)
                    .IsRequired();
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
                    .Property(m => m.SequenceNumber)
                    .HasColumnName("sequence_number")
                    .HasColumnType("integer")
                    .HasDefaultValue(0)
                    .IsRequired();
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
                entity.HasIndex(m => new { m.ConversationId, m.SequenceNumber }).IsUnique();
            });

            modelBuilder.Entity<MessageMedia>(entity =>
            {
                entity.ConfigureBaseEntity();
                entity.ToTable("message_media");
                entity
                    .Property(mm => mm.MessageId)
                    .HasColumnName("message_id")
                    .HasColumnType("uuid")
                    .IsRequired();
                entity
                    .Property(mm => mm.Url)
                    .HasColumnName("url")
                    .HasColumnType("text")
                    .IsRequired();
                entity
                    .Property(mm => mm.Type)
                    .HasColumnName("type")
                    .HasColumnType("integer")
                    .HasConversion<int>()
                    .HasDefaultValue(MediaType.Image)
                    .IsRequired();
                entity
                    .Property(mm => mm.Metadata)
                    .HasColumnName("metadata")
                    .HasColumnType("jsonb")
                    .IsRequired(false);
                entity
                    .Property(mm => mm.MessageSequence)
                    .HasColumnName("message_sequence")
                    .HasColumnType("integer")
                    .IsRequired();
                entity
                    .Property(mm => mm.IndexInMessage)
                    .HasColumnName("index_in_message")
                    .HasColumnType("integer")
                    .IsRequired();
                entity
                    .HasOne(mm => mm.Message)
                    .WithMany(m => m.Media)
                    .HasForeignKey(mm => mm.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(mm => mm.MessageId);
            });
        }
    }
}
