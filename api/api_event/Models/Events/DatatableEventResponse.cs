using api_event.EntityFrameworks.Entities;

namespace api_event.Models.Events
{
    public class DatatableEventResponse : DetailEventResponse
    {
        public DatatableEventResponse(EventEntity entity) : base(entity) { }
    }
}
