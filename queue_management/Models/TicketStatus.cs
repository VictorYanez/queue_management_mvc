using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("TicketStatus")]
    public class TicketStatus
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketStatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string? StatusName { get; set; }

        // Definición de Relaciones & propiedad de navegacion
        public virtual ICollection<TicketStatusAssignment> TicketStatusAssignments { get; set; } = new List<TicketStatusAssignment>();

        // Campos de Auditoría
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        //public virtual User CreatedByUser { get; set; }  // Propiedad de navegacion
        public DateTime CreatedAt { get; set; }

        [ForeignKey("ModifiedByUser")]
        public int? ModifiedBy { get; set; }
        //public virtual User ModifiedByUser { get; set; }  // Propiedad de navegacion
        public DateTime? ModifiedAt { get; set; }

        [Timestamp] // Esto es para control de concurrencia en SQL Server
        public byte[]? RowVersion { get; set; }
    }
}
