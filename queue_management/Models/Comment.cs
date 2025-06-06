using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Comments")]
    public class Comment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id del Comentario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int CommentId { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Comentario")]
        public DateTime DateTime { get; set; }

        [StringLength(200, MinimumLength = 5)]
        [Display(Name = "Comentario")]
        public string? CommentText { get; set; }

        // Definición de Relaciones & propiedad de navegacion
        [ForeignKey("UserId")]
        [Display(Name = "Usuario del Comentario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int UserId { get; set; }

        [ForeignKey("ServiceId")]
        [Display(Name = "Servicio de Comentario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ServiceId { get; set; }

        // public virtual User User { get; set; }
        [ForeignKey("ServiceId")]
        [Display(Name = "Servicio de Comentario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public virtual Service? Service { get; set; }

        [ForeignKey("TicketId")]
        [Display(Name = "Ticket de Comentario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int TicketId { get; set; } // Clave foránea

        [Display(Name = "Tickets")]
        public Ticket? Ticket { get; set; } // Propiedad de navegación

        // ------- Campos de Auditoría -----------------------------------------------
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
