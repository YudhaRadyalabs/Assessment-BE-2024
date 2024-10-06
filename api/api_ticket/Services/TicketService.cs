using api_ticket.EntityFrameworks.Contexts;
using api_ticket.EntityFrameworks.Entities;
using api_ticket.Models.Tickets;
using infrastructures.Services.Base;
using NATS.Client.Core;
using Newtonsoft.Json;

namespace api_ticket.Services
{
    public interface ITicketService : IBaseService
    {
        Task<TicketEntity> Create(CreateTicketRequest model);
    }

    public class TicketService : BaseService, ITicketService
    {
        private readonly AppDbContext _appDbContext;
        private readonly NatsConnection _natsConnection;

        public TicketService(AppDbContext appDbContext, NatsConnection natsConnection)
        {
            _appDbContext = appDbContext;
            _natsConnection = natsConnection;
        }

        public async Task<TicketEntity> Create(CreateTicketRequest model)
        {
            TicketEntity entity = new TicketEntity();
            model.MapToEntity(entity);
            await _appDbContext.Set<TicketEntity>().AddAsync(entity);
            await _appDbContext.SaveChangesAsync();

            await _natsConnection.PublishAsync("create_ticket", JsonConvert.SerializeObject(entity));

            return entity;
        }
    }
}
