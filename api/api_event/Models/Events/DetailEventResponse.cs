using api_event.EntityFrameworks.Entities;

namespace api_event.Models.Events
{
    public class DetailEventResponse : EventModel
    {
        public Guid Id { get; set; }
        
        public DetailEventResponse(EventEntity entity)
        {
            Id = entity.Id;
            Name = entity.Name;
            EventDate = entity.EventDate;
            Location = entity.Location;
            Total = entity.Total;
        }
    }
}
