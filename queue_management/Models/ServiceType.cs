using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;

namespace queue_management.Models
{
    [Table("ServiceTypes")]
    public class ServiceType
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [Display(Name = "Id del Tipo de Servicio")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int? ServiceTypeId { get; set; }

        [Display(Name = "Nombre del Tipo de Servicio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(150)]
        public string? ServiceTypeName { get; set; }

        // 🔥 -----   Propiedades de Navegacion   ---------------------
        [Display(Name = "Servicios")]
        public virtual ICollection<Service> Services { get; set; } = new List<Service>();

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