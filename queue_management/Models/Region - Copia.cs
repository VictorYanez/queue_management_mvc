using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Regions")]
    public class Region

    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id de la Región")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int RegionId { get; set; }

        [Display(Name = "Nombre de la Región")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(100)]
        public string? RegionName { get; set; }

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
