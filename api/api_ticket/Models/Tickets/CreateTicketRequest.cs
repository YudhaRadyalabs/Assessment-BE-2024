using api_ticket.EntityFrameworks.Entities;
using infrastructures.Helpers;
using System.ComponentModel.DataAnnotations;

namespace api_ticket.Models.Tickets
{
    public class CreateTicketRequest
    {
        public Guid EventId { get; set; }


        public void MapToEntity(TicketEntity entity)
        {
            entity.TicketNumber = StringHelper.GenerateSimpleRandomString(6);
            entity.EventId = EventId;
        }
    }
}
