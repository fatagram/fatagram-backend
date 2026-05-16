using System;
using System.IO;
using System.Threading.Tasks;
using Fatagram.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.API.Scripts
{
    public class MigrateConversationUniqueKeysCommand(AppDbContext dbContext) : ICommand
    {
        private readonly AppDbContext _dbContext = dbContext;

        public string Name => "migrate-conversation-unique-keys";

        public async Task Execute(string[] args)
        {
            var sqlPath = ResolveSqlPath();
            if (!File.Exists(sqlPath))
            {
                throw new FileNotFoundException($"SQL file not found: {sqlPath}");
            }

            var sql = await File.ReadAllTextAsync(sqlPath);
            await _dbContext.Database.ExecuteSqlRawAsync(sql);

            Console.WriteLine("MigrateConversationUniqueKeys executed successfully.");
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
                    "MigrateConversationUniqueKeys.sql"
                ),
                Path.Combine(
                    currentDirectory,
                    "..",
                    "Fatagram.Infrastructure",
                    "Sql",
                    "MigrateConversationUniqueKeys.sql"
                ),
                Path.Combine(AppContext.BaseDirectory, "Sql", "MigrateConversationUniqueKeys.sql"),
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
