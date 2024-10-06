using api_event.EntityFrameworks.Entities;
using System.ComponentModel.DataAnnotations;

namespace api_event.Models.Events
{
    public class DeleteEventRequest : EventModel
    {
        public Guid Id { get; set; }

        public void MapToEntity(EventEntity entity)
        {
            entity.Id = Id;
            entity.DeletedDate = DateTime.Now;
            entity.IsDeleted = true;
        }
    }
}
