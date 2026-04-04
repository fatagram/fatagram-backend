using System;
using System.IO;
using System.Threading.Tasks;
using Fatagram.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fatagram.API.Scripts
{
    public class SyncConvMessageCountCommand(AppDbContext dbContext) : ICommand
    {
        private readonly AppDbContext _dbContext = dbContext;

        public string Name => "sync-conv-message-count";

        public async Task Execute(string[] args)
        {
            var sqlPath = ResolveSqlPath();
            if (!File.Exists(sqlPath))
            {
                throw new FileNotFoundException($"SQL file not found: {sqlPath}");
            }

            var sql = await File.ReadAllTextAsync(sqlPath);
            await _dbContext.Database.ExecuteSqlRawAsync(sql);

            Console.WriteLine("SyncConvMessageCount executed successfully.");
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
                    "SyncConvMessageCount.sql"
                ),
                Path.Combine(
                    currentDirectory,
                    "..",
                    "Fatagram.Infrastructure",
                    "Sql",
                    "SyncConvMessageCount.sql"
                ),
                Path.Combine(AppContext.BaseDirectory, "Sql", "SyncConvMessageCount.sql"),
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
