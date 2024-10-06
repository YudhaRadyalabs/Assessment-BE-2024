using api_event.EntityFrameworks.Entities;
using System.ComponentModel.DataAnnotations;

namespace api_event.Models.Events
{
    public class CreateEventRequest : EventModel
    {
        public void MapToEntity(EventEntity entity)
        {
            entity.Name = Name;
            entity.EventDate = EventDate;
            entity.Location = Location;
            entity.Total = Total;
        }
    }
}
