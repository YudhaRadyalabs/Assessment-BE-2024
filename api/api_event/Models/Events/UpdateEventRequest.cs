using api_event.EntityFrameworks.Entities;
using System.ComponentModel.DataAnnotations;

namespace api_event.Models.Events
{
    public class UpdateEventRequest : EventModel
    {
        public Guid Id { get; set; }

        public void MapToEntity(EventEntity entity)
        {
            entity.Id = Id;
            entity.Name = Name;
            entity.EventDate = EventDate;
            entity.Location = Location;
            entity.Total = Total;
        }
    }
}
