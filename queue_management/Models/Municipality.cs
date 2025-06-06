using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using queue_management.Enums;

namespace queue_management.Models
{
    [Table("Municipalities")]
    public class Municipality
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id del Municipio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int MunicipalityId { get; set; }
 
        [Display(Name = "Nombre del Municipio")]
        [Required(ErrorMessage = "El campo Nombre del Municipio es obligatorio")]
        [StringLength(100)]
        public string? MunicipalityName { get; set; }

        // 🔥 -----  Estados de la Entidad    ----------
        [Display(Name = "Estado del Municipio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public VisibilityStatus VisibilityStatus { get; set; } = VisibilityStatus.Activo;

        [Display(Name = "Código del Municipio")]
        [StringLength(14)]
        public string CustomCode { get; set; } = null!;

        [Display(Name = "Municipio por Defecto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool IsDefault { get; set; }

        // 🔥 Definición de Relaciones & Propiedad de Navegacion  
        [ForeignKey("Country")]
        [Display(Name = "Id del País")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int CountryId { get; set; }

        [Display(Name = "Nombre del País")]
        public virtual Country Country { get; set; } = null!;

        // 🔥 -----   Entidad Departamento   ---------------------
        [ForeignKey("Department")]
        [Display(Name = "Id del Departamento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int DepartmentId { get; set; }

        [Display(Name = "Nombre del Departamento")]
        public virtual Department Department { get; set; } = null!;

        // 🔥 -----   Propiedades de Navegacion   ---------------------
        [Display(Name = "Ciudades")]
        public virtual ICollection<City> Cities { get; set; } = new List<City>();

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
