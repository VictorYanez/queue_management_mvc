using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace queue_management.Models
{
    [Table("Status")]
    public class Status
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id del Estatus")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int StatusId { get; set; }

        [Display(Name = "Nombre del Estatus")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(50)]
        public string? StatusName { get; set; }

        // 🔥 -----   Propiedades de Navegacion   ---------------------
        [Display(Name = "Appointments")]
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

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


