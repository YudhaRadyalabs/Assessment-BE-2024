using System.ComponentModel.DataAnnotations;

namespace api_event.Models.Events
{
    public class EventModel
    {
        public string Name { get; set; } = string.Empty;
        public DateTime EventDate { get; set; }
        public string Location { get; set; } = string.Empty;
        public int Total { get; set; } = 0;
    }
}
