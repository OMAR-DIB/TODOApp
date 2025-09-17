using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDo.Data.Entities.Base
{
    public abstract class BaseEntity
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? UpdatedAt { get; set; }
        
        public bool IsDeleted { get; set; }

        // Make this non-null by default to avoid CS8618 warning
        public string CreatedBy { get; set; } = "System";
    }
}
