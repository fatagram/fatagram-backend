using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Fatagram.API.Scripts
{
    public static class ScriptRunner
    {
        public static bool IsScriptMode(string[] args)
        {
            return args.Length > 0
                && string.Equals(args[0], "script", StringComparison.OrdinalIgnoreCase);
        }

        public static async Task<int> RunAsync(IServiceProvider serviceProvider, string[] args)
        {
            if (args.Length < 2)
            {
                Console.WriteLine(
                    "Usage: dotnet run --project Fatagram.API -- script <command> [args...]"
                );
                return 1;
            }

            var commandName = args[1];
            var commandArgs = args.Skip(2).ToArray();

            using var scope = serviceProvider.CreateScope();
            var commands = scope.ServiceProvider.GetServices<ICommand>().ToList();

            if (commands.Count == 0)
            {
                Console.WriteLine(
                    "No script commands registered. Add ICommand implementations in Fatagram.API/Scripts."
                );
                return 1;
            }

            var command = commands.FirstOrDefault(c =>
                string.Equals(c.Name, commandName, StringComparison.OrdinalIgnoreCase)
            );

            if (command is null)
            {
                Console.WriteLine($"Script '{commandName}' not found.");
                Console.WriteLine("Available scripts:");
                foreach (var item in commands.OrderBy(c => c.Name))
                {
                    Console.WriteLine($"- {item.Name}");
                }

                return 1;
            }

            await command.Execute(commandArgs);
            return 0;
        }
    }
}
