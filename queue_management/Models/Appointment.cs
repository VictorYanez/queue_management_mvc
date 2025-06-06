using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Appointments")]
    public class Appointment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id de la Cita")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int AppointmentId { get; set; }

        [Display(Name = "Número de DUI")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(11, MinimumLength = 5)]
        public string? DUI { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de la Cita")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public DateTime DateTime { get; set; }

        [ForeignKey("ServiceId")]
        [Display(Name = "Servicio Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int ServiceId { get; set; }

        [Display(Name = "Nombre del Servicio")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public virtual Service? Service { get; set; }

        [Display(Name = "Observaciones de la Cita")]
        [StringLength(200, MinimumLength = 5)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Observations { get; set; }

        [ForeignKey("StatusId")]
        [Display(Name = "Id Estado")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int StatusId { get; set; }
        public virtual Status? Status { get; set; }


        // Definición de Relaciones & propiedad de navegacion
        //----------------------------------------------------
        [ForeignKey("UserId")]
        [Display(Name = "Id Usuario")]
        public int UserId { get; set; }
        //public virtual User? User { get; set; }

        [ForeignKey("AgentId")]
        [Display(Name = "Id Agente")]
        public int AgentId { get; set; }
        public virtual Agent? Agent { get; set; }

        [ForeignKey("LocationId")]
        [Display(Name = "Id Ubicación")]
        public int LocationId { get; set; }
        public virtual Location? Location { get; set; }

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
