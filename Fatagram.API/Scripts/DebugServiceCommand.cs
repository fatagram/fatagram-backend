using System;
using System.Threading.Tasks;
using Fatagram.Application.Dtos;
using Fatagram.Application.Dtos.Filter;
using Fatagram.Application.Services.ConversationServices.Interfaces;
using Fatagram.Domain.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Fatagram.API.Scripts
{
    public class DebugServiceCommand : ICommand
    {
        private readonly IServiceProvider _serviceProvider;

        public DebugServiceCommand(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name => "debug-service";

        public async Task Execute(string[] args)
        {
            var userId = Guid.Parse("c08376e8-71c5-43c7-8000-6c01f42080b6");
            using var scope = _serviceProvider.CreateScope();
            var service = scope.ServiceProvider.GetRequiredService<IConversationService>();

            try
            {
                var filter = new CursorFilter<DateTime> { Limit = 100 };
                var res = await service.GetAllAsync(userId, filter);
                if (res.Data?.Items != null)
                {
                    foreach (var c in res.Data.Items)
                    {
                        Console.WriteLine(
                            $"ConvId: {c.Id}, Name: {c.Name}, Avatar: {c.AvatarUrl}, OtherUserId: {c.OtherUserId}"
                        );
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("EXCEPTION CAUGHT: " + ex.ToString());
            }
        }
    }
}
