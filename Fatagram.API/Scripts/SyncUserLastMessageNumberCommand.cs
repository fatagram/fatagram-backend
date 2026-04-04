using System;
using System.IO;
using System.Threading.Tasks;
using Fatagram.Infrastructure.Data;

namespace Fatagram.API.Scripts
{
    public class SyncUserLastMessageNumberCommand(AppDbContext dbContext) : ICommand
    {
        private readonly AppDbContext _dbContext = dbContext;

        public string Name => "sync-user-last-message-number";

        public async Task Execute(string[] args)
        {
            var sqlPath = ResolveSqlPath();
            if (!File.Exists(sqlPath))
            {
                throw new FileNotFoundException($"SQL file not found: {sqlPath}");
            }

            var sql = await File.ReadAllTextAsync(sqlPath);
            await _dbContext.Database.ExecuteSqlRawAsync(sql);

            Console.WriteLine("SyncUserLastMessageNumber executed successfully.");
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
                    "SyncUserLastMessageNumber.sql"
                ),
                Path.Combine(
                    currentDirectory,
                    "..",
                    "Fatagram.Infrastructure",
                    "Sql",
                    "SyncUserLastMessageNumber.sql"
                ),
                Path.Combine(AppContext.BaseDirectory, "Sql", "SyncUserLastMessageNumber.sql"),
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
