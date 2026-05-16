using System;
using System.IO;
using System.Threading.Tasks;
using Fatagram.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.API.Scripts
{
    public class RebuildConversationSearchTextCommand(AppDbContext dbContext) : ICommand
    {
        private readonly AppDbContext _dbContext = dbContext;

        public string Name => "rebuild-conversation-search-text";

        public async Task Execute(string[] args)
        {
            var sqlPath = ResolveSqlPath();
            if (!File.Exists(sqlPath))
            {
                throw new FileNotFoundException($"SQL file not found: {sqlPath}");
            }

            var sql = await File.ReadAllTextAsync(sqlPath);
            await _dbContext.Database.ExecuteSqlRawAsync(sql);

            Console.WriteLine("RebuildConversationSearchText executed successfully.");
            Console.WriteLine($"SQL: {sqlPath}");
        }

        private static string ResolveSqlPath()
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var candidatePaths = new[]
            {
                Path.Combine(
                    currentDirectory,
                    "Fatagram.Infrastructure",
                    "Sql",
                    "RebuildConversationSearchText.sql"
                ),
                Path.Combine(
                    currentDirectory,
                    "..",
                    "Fatagram.Infrastructure",
                    "Sql",
                    "RebuildConversationSearchText.sql"
                ),
                Path.Combine(AppContext.BaseDirectory, "Sql", "RebuildConversationSearchText.sql"),
            };

            foreach (var path in candidatePaths)
            {
                var fullPath = Path.GetFullPath(path);
                if (File.Exists(fullPath))
                {
                    return fullPath;
                }
            }

            return Path.GetFullPath(candidatePaths[^1]);
        }
    }
}
