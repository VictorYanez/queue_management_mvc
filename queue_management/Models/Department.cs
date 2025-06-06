using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using queue_management.Enums;

namespace queue_management.Models
{
    [Table("Departments")]
    public class Department
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [Display(Name = "Id del Departamento")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int DepartmentId { get; set; }

        [Display(Name = "Nombre del Departamento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(100)]
        public string? DepartmentName { get; set; }

        // 🔥 -----  Estados de la Entidad    ----------
        [Display(Name = "Estado del Departamento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public VisibilityStatus VisibilityStatus { get; set; } = VisibilityStatus.Activo;

        [Display(Name = "Código del Departamento")]
        [StringLength(14)]
        public string CustomCode { get; set; } = null!;

        [Display(Name = "Departamento por Defecto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool IsDefault { get; set; }

        // 🔥 Definición de Relaciones & Propiedad de Navegacion  
        [ForeignKey("Country")]
        [Display(Name = "Id del País")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int CountryId { get; set; }

        [Display(Name = "Nombre del País")]
        public virtual Country Country { get; set; } = null!;

        // ----  🔥 -----   Propiedades de Navegacion   ---------------------  
        [Display(Name = "Unidades")]
        public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();

        [Display(Name = "Municipios")]
        public virtual ICollection<Municipality> Municipalities { get; set; } = new List<Municipality>();

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
