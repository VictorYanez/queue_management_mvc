using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Tickets")]
    public class Ticket
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id del Ticket")] 
        public int TicketId { get; set; }

        [Display(Name = "Fecha del Ticket")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [CustomValidation(typeof(Rating), "ValidateEvaluationDate")]
        public DateTime CreationDateTime { get; set; }

        [Display(Name = "Codigo del Ticket")]
        public string? TicketCode { get; set; }

        // 🔥 Definición de Relaciones & Propiedad de Navegacion  -----
        [ForeignKey("Service")]
        [Display(Name = "Id del Servicio")]
        public int ServiceId { get; set; }

        [ForeignKey("Queue")]
        [Display(Name = "Id de la Cola")]
        public int QueueId { get; set; }

        [ForeignKey("User")]
        [Display(Name = "Id del Ticket")]
        public int UserId { get; set; }

        // Definición de Relaciones & propiedad de navegacion
        [ForeignKey("TicketStatus")]
        public int TicketStatusId { get; set; }

        // 🔥 Definición de Propiedad de Navegacion  -----
        //public virtual User? User { get; set; }
        public virtual Service? Service { get; set; }
        public virtual Queue? Queue { get; set; }
        public virtual TicketStatus? TicketStatus { get; set; }

        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();  // Colección de comentarios relacionados
        public virtual ICollection<TicketStatusAssignment> TicketStatusAssignments { get; set; } = new List<TicketStatusAssignment>();


        // 🔥 -----  Campos de Auditoría    ------------------------------------------
        [ForeignKey("CreatedByUser")]
        [ScaffoldColumn(false)]
        public int CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime CreatedAt { get; set; }

        [ForeignKey("ModifiedByUser")]
        [ScaffoldColumn(false)]
        public int? ModifiedBy { get; set; }

        [ScaffoldColumn(false)]
        public DateTime? ModifiedAt { get; set; }

        [Timestamp]
        public byte[]? RowVersion { get; set; }

    }

}
