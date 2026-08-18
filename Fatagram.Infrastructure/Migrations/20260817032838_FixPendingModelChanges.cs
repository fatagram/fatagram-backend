using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Fatagram.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixPendingModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // No-op: pinned_at column already added by 20260816082355_AddPinnedAtToConversationParticipant
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // No-op
        }
    }
}
