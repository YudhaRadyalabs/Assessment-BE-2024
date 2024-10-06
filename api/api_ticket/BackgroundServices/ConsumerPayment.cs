using api_ticket.EntityFrameworks.Contexts;
using api_ticket.EntityFrameworks.Entities;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace api_ticket.BackgroundServices
{
    public class ConsumerPayment : BackgroundService
    {
        private readonly NatsConnection _natsConnection;
        private readonly IServiceScopeFactory _scopeFactory;

        public ConsumerPayment(NatsConnection natsConnection, IServiceScopeFactory scopeFactory)
        {
            _natsConnection = natsConnection;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("confiramtion_payment"))
                {
                    var data = JObject.Parse(item.Data);
                    if (data != null)
                        await ValidateTicket(data);
                }
            });
        }

        private async Task ValidateTicket(JObject data)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var _appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    var id = Guid.Parse(data["Id"].ToString());
                    var isSuccess = bool.Parse(data["IsSuccess"].ToString());

                    var entity = _appDbContext.Set<TicketEntity>().FirstOrDefault(x => x.Id == id);
                    if (entity != null)
                    {
                        if(isSuccess)
                            entity.Status = TicketStatus.Paid;
                        else
                            entity.Status = TicketStatus.FailedPaid;
                        _appDbContext.Set<TicketEntity>().Update(entity);
                        await _appDbContext.SaveChangesAsync();

                        //send notification
                        var user = await _appDbContext.Set<UserEntity>().FirstOrDefaultAsync(x => x.Id == entity.CreatedBy);
                        if (user != null)
                        {
                            string message = "";
                            if (isSuccess)
                                message = $"Dear {user.Fullname}, <br/> Ticket yang anda pesan berhasil dibayar. <br/>";
                            else
                                message = $"Dear {user.Fullname}, <br/> Ticket yang anda pesan gagal dibayar.";
                            string email = user.Email;

                            await _natsConnection.PublishAsync("notification_payment", JsonConvert.SerializeObject(new
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
