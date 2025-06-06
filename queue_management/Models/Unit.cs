using queue_management.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Units")]
    public class Unit
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id de Unidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int UnitId { get; set; }

        [Required(ErrorMessage = "El campo {0} es requerido")]
        [Display(Name = "Nombre de Unidad")]
        public String? UnitName { get; set; }

        [StringLength(200)]
        [Display(Name = "Descripción de Unidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? UnitDescription { get; set; }

        // 🔥 -----  Estados de la Entidad    ----------
        [Display(Name = "Estado de la Unidad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public VisibilityStatus VisibilityStatus { get; set; } = VisibilityStatus.Activo;

        // 🔥 Definición de Relaciones & Propiedad de Navegacion  
        [ForeignKey("AreaId")]
        [Display(Name = "Id de Área")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int AreaId { get; set; }

        [Display(Name = "Area")]
        public virtual Area Area { get; set; } = null!; //Perdonar el nulo?  

        // 🔥 -----   Propiedades de Navegacion   ---------------------
        public virtual ICollection<Agent> Agents { get; set; } = new List<Agent>();
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
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
