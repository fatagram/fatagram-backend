using System;
using System.Threading.Tasks;

namespace Fatagram.API.Scripts
{
    public class PingCommand : ICommand
    {
        public string Name => "ping";

        public Task Execute(string[] args)
        {
            Console.WriteLine("pong");
            Console.WriteLine($"utc: {DateTime.UtcNow:O}");
            if (args.Length > 0)
            {
                Console.WriteLine($"args: {string.Join(' ', args)}");
            }

            return Task.CompletedTask;
        }
    }
}
