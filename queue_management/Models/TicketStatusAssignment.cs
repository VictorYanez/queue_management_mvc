using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("TicketStatusAssignments")]
    public class TicketStatusAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TicketStatusAssignmentId { get; set; }

        [ForeignKey("Ticket")]
        public int TicketId { get; set; }

        [ForeignKey("TicketStatus")]
        public int TicketStatusId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        // Definición de Relaciones & propiedad de navegacion
        public virtual Ticket? Ticket { get; set; }
        public virtual TicketStatus? TicketStatus { get; set; }

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
