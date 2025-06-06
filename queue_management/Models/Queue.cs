using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace queue_management.Models
{
    [Table("Queues")]
    public class Queue
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id de la cola")]
        public int QueueId { get; set; }

        [Display(Name = "Nombre de la Cola")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(200)]
        public string? QueueName { get; set; }

        [Display(Name = "Descripción de la Cola")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(255)]
        public string? Description { get; set; }

        [Display(Name = "Tipo de la Cola")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int? QueueType { get; set; }

        [Display(Name = "Forma de asignación Cola")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(100)]
        public int? AssignmentStrategy { get; set; }

        // 🔥 Definición de Relaciones & Propiedad de Navegacion  -----
        [ForeignKey("Service")]
        public int ServiceId { get; set; }  // Clave foránea hacia Service
        public Service? Service { get; set; }  // Propiedad de navegación

        [ForeignKey("QueueStatus")]
        public int QueueStatusId { get; set; }
        public virtual QueueStatus? QueueStatus { get; set; }

        [ForeignKey("Location")]
        public int LocationId { get; set; }  // Clave foránea hacia Location
        public virtual Location? Location { get; set; }  // Propiedad de navegación

        [ForeignKey("Unit")]
        public int? UnitId { get; set; }  // Clave foránea hacia Unit (opcional)
        public virtual Unit? Unit { get; set; }  // Propiedad de navegación

        public virtual ICollection<QueueAssignment> QueueAssignments { get; set; } = new List<QueueAssignment>();
        public virtual ICollection<QueueStatusAssignment> QueueStatusAssignments { get; set; } = new List<QueueStatusAssignment>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();

        // 🔥 -----  Campos de Auditoría    ------------------------------------------
        // ------- Información de Datos en Log (Registro – Creación) -----------------
        [ForeignKey("CreatedByUser")]
        [ScaffoldColumn(false)]
        public int CreatedBy { get; set; }
        // [ScaffoldColumn(false)]
        // public virtual User CreatedByUser { get; set; }  // Propiedad de navegacion
        [ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; }

        // ------- Información de Datos en Log (Registro – Edición) -----------------
        [ForeignKey("ModifiedByUser")]
        [ScaffoldColumn(false)]
        public int? ModifiedBy { get; set; }
        //[ScaffoldColumn(false)]
        //public virtual User ModifiedByUser { get; set; }  // Propiedad de navegacion
        [ScaffoldColumn(false)]
        public DateTime? ModifiedAt { get; set; }

        // ------- Esto es para control de concurrencia en SQL Server -----------------
        [Timestamp]
        public byte[]? RowVersion { get; set; }

    }
}
