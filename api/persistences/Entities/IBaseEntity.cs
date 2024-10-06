using System.ComponentModel.DataAnnotations;

namespace persistences.Entities
{
    public interface IBaseEntity
    {
        public Guid Id { get; set; }
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? DeletedBy { get; set; }
        public string CreatedByName { get; set; }
        public string UpdatedByName { get; set; }
        public string DeletedByName { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
    public class BaseEntity
    {
        [Key]
        public Guid Id { get; set; } = Guid.NewGuid();
        public Guid? CreatedBy { get; set; }
        public Guid? UpdatedBy { get; set; }
        public Guid? DeletedBy { get; set; }
        public string CreatedByName { get; set; } = "SYSTEM";
        public string UpdatedByName { get; set; } = "";
        public string DeletedByName { get; set; } = "";
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime? UpdatedDate { get; set; }
        public DateTime? DeletedDate { get; set; }
        public bool IsDeleted { get; set; } = false;
    }
}
