using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Fatagram.API.Scripts
{
    public class SyncMessageMediaIndexInMessageAndMessageSequence(AppDbContext context) : ICommand
    {
        private readonly AppDbContext _dbContext = context;

        public string Name => "sync-message-media-index-in-message-and-message-sequence";

        public async Task Execute(string[] args)
        {
            Console.WriteLine("🚀 Starting FOOLPROOF self-healing script for Media...");

            // 1. Lấy danh sách các MessageId ĐANG CÓ media (Tránh quét toàn bộ DB)
            var messageIdsWithMedia = await _dbContext
                .MessageMedias.Select(m => m.MessageId)
                .Distinct()
                .ToListAsync();

            int updatedCount = 0;
            int processedMessages = 0;

            // 2. Duyệt qua từng MessageId (Tuyệt đối không bị lẫn lộn dữ liệu)
            foreach (var msgId in messageIdsWithMedia)
            {
                // Lấy đích danh SequenceNumber gốc từ bảng Message
                var parentSequence = await _dbContext
                    .Messages.Where(m => m.Id == msgId)
                    .Select(m => m.SequenceNumber)
                    .FirstOrDefaultAsync();

                // Nếu vì lý do nào đó Message gốc bị xóa nhưng Media còn sót (Orphan data)
                if (parentSequence == 0)
                    continue;

                // Lấy toàn bộ media của ĐÚNG message này
                var medias = await _dbContext
                    .MessageMedias.Where(m => m.MessageId == msgId)
                    .OrderBy(m => m.CreatedAt)
                    .ThenBy(m => m.Id) // Tie-breaker chống trùng thời gian
                    .ToListAsync();

                for (int i = 0; i < medias.Count; i++)
                {
                    medias[i].MessageSequence = parentSequence;
                    medias[i].IndexInMessage = i;
                    updatedCount++;
                }

                // 3. Lưu trực tiếp thay đổi của MessageId này xuống DB
                await _dbContext.SaveChangesAsync();

                // 4. QUAN TRỌNG NHẤT (THE MOST IMPORTANT):
                // Xóa Change Tracker để EF Core giải phóng RAM và không bị "nhớ nhầm" object cũ
                _dbContext.ChangeTracker.Clear();

                processedMessages++;
                if (processedMessages % 100 == 0)
                {
                    Console.WriteLine(
                        $"✅ Processed {processedMessages} messages (Fixed {updatedCount} media)..."
                    );
                }
            }

            Console.WriteLine(
                $"\n✨ Done! Successfully healed {updatedCount} media records across {processedMessages} messages."
            );
        }
    }
}
