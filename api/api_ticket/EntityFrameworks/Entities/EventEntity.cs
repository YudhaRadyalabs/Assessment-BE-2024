using persistences.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_ticket.EntityFrameworks.Entities
{
    [Table("ms_event")]
    public class EventEntity : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [Required]
        public DateTime EventDate { get; set; }
        [Required]
        public string Location { get; set; } = string.Empty;
    }
}
