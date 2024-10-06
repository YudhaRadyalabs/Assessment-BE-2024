using api_event.EntityFrameworks.Contexts;
using api_event.EntityFrameworks.Entities;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using Newtonsoft.Json.Linq;

namespace api_event.BackgroundServices
{
    public class ConsumerTicket : BackgroundService
    {
        private readonly NatsConnection _natsConnection;
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsumerTicket(NatsConnection natsConnection, IServiceScopeFactory scopeFactory)
        {
            _natsConnection = natsConnection;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("create_ticket"))
                {
                    var data = JObject.Parse(item.Data);
                    if (data != null)
                    {
                        if (await CreateTicket(data))
                        {
                            await _natsConnection.PublishAsync("validate_ticket", new
                            {
                                Id = Guid.Parse(data["Id"].ToString()),
                                IsValid = true
                            });
                        }
                        else
                        {
                            await _natsConnection.PublishAsync("validate_ticket", new
                            {
                                Id = Guid.Parse(data["Id"].ToString()),
                                IsValid = false
                            });
                        }
                    }
                }
            });
        }

        private async Task<bool> CreateTicket(JObject data)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var eventId = Guid.Parse(data["EventId"].ToString());
                    var eventEntity = await _appDbContext.Set<EventEntity>().FirstOrDefaultAsync(x => x.Id == eventId);

                    if (eventEntity == null)
                    {
                        return false;
                    }
                    else 
                    {
                        if (eventEntity.Total - 1 < 0)
                        {
                            return false;
                        }
                        else
                        {
                            eventEntity.Total = eventEntity.Total - 1;
                            _appDbContext.Set<EventEntity>().Update(eventEntity);
                            await _appDbContext.SaveChangesAsync();
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
