using api_ticket.EntityFrameworks.Contexts;
using api_ticket.EntityFrameworks.Entities;
using NATS.Client.Core;
using Newtonsoft.Json.Linq;

namespace api_ticket.BackgroundServices
{
    public class ConsumerUser : BackgroundService
    {
        private readonly NatsConnection _natsConnection;
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsumerUser(NatsConnection natsConnection, IServiceScopeFactory scopeFactory)
        {
            _natsConnection = natsConnection;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("register_user"))
                {
                    var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<UserEntity>(item.Data);
                    if (entity != null)
                        await UpsertUser(entity);
                }
            });
        }

        private async Task UpsertUser(UserEntity entity)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (_appDbContext.Set<UserEntity>().Any(x => x.Id == entity.Id))
                        _appDbContext.Set<UserEntity>().Update(entity);
                    else
                        await _appDbContext.Set<UserEntity>().AddAsync(entity);


                    await _appDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
