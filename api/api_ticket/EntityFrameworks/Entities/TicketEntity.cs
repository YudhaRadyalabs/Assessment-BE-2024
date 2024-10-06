using persistences.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api_ticket.EntityFrameworks.Entities
{
    [Table("ms_ticket")]
    public class TicketEntity : BaseEntity
    {
        [Required]
        [MaxLength(50)]
        public string TicketNumber { get; set; } = string.Empty;
        [Required]
        public TicketStatus Status { get; set; } = TicketStatus.Initial;
        [Required]
        public Guid EventId { get; set; }

        [ForeignKey(nameof(EventId))]
        public virtual EventEntity eventEntity { get; set; }
    }

    public enum TicketStatus
    {
        Initial,
        OutOfStock,
        WaitingPayment,
        Paid,
        FailedPaid,
    }
}