using api_ticket.EntityFrameworks.Contexts;
using api_ticket.EntityFrameworks.Entities;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api_ticket.BackgroundServices
{
    public class ConsumerEvent : BackgroundService
    {
        private readonly NatsConnection _natsConnection;
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsumerEvent(NatsConnection natsConnection, IServiceScopeFactory scopeFactory)
        {
            _natsConnection = natsConnection;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("create_event"))
                {
                    var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<EventEntity>(item.Data);
                    if (entity != null)
                        await UpsertEvent(entity);
                }
            });

            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("update_event"))
                {
                    var entity = Newtonsoft.Json.JsonConvert.DeserializeObject<EventEntity>(item.Data);
                    if (entity != null)
                        await UpsertEvent(entity);
                }
            });

            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("delete_event"))
                {
                    var id = Newtonsoft.Json.JsonConvert.DeserializeObject<Guid>(item.Data);
                    if (id != null)
                        await DeleteEvent(id);
                }
            });

            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("validate_ticket"))
                {
                    var data = JObject.Parse(item.Data);
                    if (data != null)
                        await ValidateTicket(data);
                }
            });
        }

        private async Task UpsertEvent(EventEntity entity)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    if (_appDbContext.Set<EventEntity>().Any(x => x.Id == entity.Id))
                        _appDbContext.Set<EventEntity>().Update(entity);
                    else
                        await _appDbContext.Set<EventEntity>().AddAsync(entity);


                    await _appDbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task DeleteEvent(Guid id)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var entity = _appDbContext.Set<EventEntity>().FirstOrDefault(x => x.Id == id);
                    if (entity != null)
                    {
                        entity.IsDeleted = true;
                        entity.DeletedDate = DateTime.Now;
                        _appDbContext.Set<EventEntity>().Update(entity);
                        await _appDbContext.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private async Task ValidateTicket(JObject data)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var id = Guid.Parse(data["Id"].ToString());
                    var isValid = bool.Parse(data["IsValid"].ToString());

                    var entity = _appDbContext.Set<TicketEntity>().FirstOrDefault(x => x.Id == id);
                    if (entity != null)
                    {
                        if(isValid)
                            entity.Status = TicketStatus.WaitingPayment;
                        else
                            entity.Status = TicketStatus.OutOfStock;
                        _appDbContext.Set<TicketEntity>().Update(entity);
                        await _appDbContext.SaveChangesAsync();

                        //send notification
                        var user = await _appDbContext.Set<UserEntity>().FirstOrDefaultAsync(x => x.Id == entity.CreatedBy);
                        if (user != null)
                        {
                            string message = "";
                            if (isValid)
                                message = $"Dear {user.Fullname}, <br/> Ticket yang anda pesan menunggu untuk dibayar. <br/>";
                            else
                                message = $"Dear {user.Fullname}, <br/> Pemesanan ticket yang anda pesan tidak dapat dilanjutan.";
                            string email = user.Email;

                            await _natsConnection.PublishAsync("notification_valid_ticket", JsonConvert.SerializeObject(new
                            {
                                message,
                                email
                            }));
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
