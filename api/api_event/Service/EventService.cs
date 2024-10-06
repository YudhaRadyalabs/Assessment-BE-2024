using api_event.EntityFrameworks.Contexts;
using api_event.EntityFrameworks.Entities;
using api_event.Models.Events;
using infrastructures.Models.Paginations;
using infrastructures.Services.Base;
using Microsoft.EntityFrameworkCore;
using NATS.Client.Core;
using Newtonsoft.Json;

namespace api_event.Service
{
    public interface IEventService : IBaseService
    {
        Task<PaginationResponse<DatatableEventResponse>> Datatable(PaginationRequest request);
        Task<DetailEventResponse?> GetById(Guid id);
        Task<EventEntity> Create(CreateEventRequest model);
        Task<EventEntity> Update(UpdateEventRequest model);
        Task<EventEntity> Delete(DeleteEventRequest model);
    }

    public class EventService : BaseService, IEventService
    {
        private readonly AppDbContext _appDbContext;
        private readonly NatsConnection _natsConnection;

        public EventService(AppDbContext appDbContext, NatsConnection natsConnection)
        {
            _appDbContext = appDbContext;
            _natsConnection = natsConnection;
        }

        public async Task<PaginationResponse<DatatableEventResponse>> Datatable(PaginationRequest request)
        {
            var query = _appDbContext.Set<EventEntity>().Where(x => !x.IsDeleted).AsQueryable();

            if (request.GeneralSearch != "")
            {
                query = query.Where(x => x.Name.ToLower().Contains(request.GeneralSearch.ToLower()));
            }

            foreach (var item in request.Searches)
            {
                switch (item.Field.ToLower())
                {
                    case "name":
                        query = query.Where(x => x.Name != null && x.Name == item.Value);
                        break;
                    default:
                        break;
                }
            }

            foreach (var item in request.Sorts)
            {
                switch (item.Field.ToLower())
                {
                    case "name":
                        query = item.Order.ToLower() == "asc" ? query.OrderBy(x => x.Name) : query.OrderByDescending(x => x.Name);
                        break;
                }
            }

            var count = await query.CountAsync();
            var data = await query.Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new DatatableEventResponse(x))
                .ToListAsync();

            return new PaginationResponse<DatatableEventResponse>(data,count, request.PageNumber, request.PageSize);
        }

        public async Task<DetailEventResponse?> GetById(Guid id)
        {
            var data = await _appDbContext.Set<EventEntity>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted);
            if (data != null)
                return new DetailEventResponse(data);
            else
                return null;
        }

        public async Task<EventEntity> Create(CreateEventRequest model)
        {
            EventEntity entity = new EventEntity();
            model.MapToEntity(entity);
            await _appDbContext.Set<EventEntity>().AddAsync(entity);
            await _appDbContext.SaveChangesAsync();

            await _natsConnection.PublishAsync("create_event", JsonConvert.SerializeObject(entity));

            return entity;
        }

        public async Task<EventEntity> Update(UpdateEventRequest model)
        {
            EventEntity entity = await _appDbContext.Set<EventEntity>().FirstOrDefaultAsync(x => x.Id == model.Id);
            model.MapToEntity(entity);
            _appDbContext.Set<EventEntity>().Update(entity);
            await _appDbContext.SaveChangesAsync();

            await _natsConnection.PublishAsync("update_event", JsonConvert.SerializeObject(entity));

            return entity;
        }

        public async Task<EventEntity> Delete(DeleteEventRequest model)
        {
            EventEntity entity = await _appDbContext.Set<EventEntity>().FirstOrDefaultAsync(x => x.Id == model.Id);
            model.MapToEntity(entity);
            _appDbContext.Set<EventEntity>().Update(entity);
            await _appDbContext.SaveChangesAsync();

            await _natsConnection.PublishAsync("delete_event", JsonConvert.SerializeObject(entity.Id));

            return entity;
        }
    }
}
