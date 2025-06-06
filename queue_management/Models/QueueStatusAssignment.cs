using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("QueueStatusAssignments")]
    public class QueueStatusAssignment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QueueStatusAssignmentId { get; set; }

        [ForeignKey("Queue")]
        public int QueueId { get; set; }

        [ForeignKey("QueueStatus")]
        public int QueueStatusId { get; set; }

        [Required]
        public DateTime DateTime { get; set; }

        // Definición de Relaciones & propiedad de navegacion
        public virtual Queue? Queue { get; set; }
        public virtual QueueStatus? QueueStatus { get; set; }

        // Campos de Auditoría
        // -----------------------------------------------
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
