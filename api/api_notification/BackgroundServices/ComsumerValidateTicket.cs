using api_notification.Services;
using NATS.Client.Core;
using Newtonsoft.Json.Linq;

namespace api_notification.BackgroundServices
{
    public class ComsumerValidateTicket : BackgroundService
    {
        private readonly NatsConnection _natsConnection;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly SmtpService _smtpService;
        public ComsumerValidateTicket(NatsConnection natsConnection, SmtpService smtpService, IServiceScopeFactory scopeFactory)
        {
            _natsConnection = natsConnection;
            _scopeFactory = scopeFactory;
            _smtpService = smtpService;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("notification_valid_ticket"))
                {
                    var data = JObject.Parse(item.Data);

                    _smtpService.SendEmailAsync(new Models.MessageEmail()
                    {
                        Subject = "Konfirmasi Tiket",
                        Body = data["message"].ToString(),
                        To = data["email"].ToString(),

                    }); ;
                }
            });

            Task.Run(async () =>
            {
                await foreach (var item in _natsConnection.SubscribeAsync<string>("notification_payment"))
                {
                    var data = JObject.Parse(item.Data);

                    _smtpService.SendEmailAsync(new Models.MessageEmail()
                    {
                        Subject = "Konfirmasi Tiket",
                        Body = data["message"].ToString(),
                        To = data["email"].ToString(),

                    }); ;
                }
            });
        }
    }
}
