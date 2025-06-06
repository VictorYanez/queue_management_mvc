using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("ServiceWindows")]
    public class ServiceWindow
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WindowId { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }

        [Required]
        public int Number { get; set; }

        [ForeignKey("Service")]
        public int ServiceId { get; set; }

        // Definición de Relaciones & propiedad de navegacion
        public virtual Location? Location { get; set; } 
        public virtual Service? Service { get; set; }

        // Campos de Auditoría
        // -------------------------------------------
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
