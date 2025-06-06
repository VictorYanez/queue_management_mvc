using queue_management.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Areas")]
    public class Area
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id del Área")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int AreaId { get; set; }

        [Display(Name = "Nombre de Área")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public String? AreaName { get; set; }

        [StringLength(200)]
        [Display(Name = "Descripción de Área")]
        public string? AreaDescription { get; set; }

        // 🔥 -----  Estados de la Entidad    ----------
        [Display(Name = "Estado del Área")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public VisibilityStatus VisibilityStatus { get; set; } = VisibilityStatus.Activo;

        // ----  🔥 -----   Propiedades de Navegacion   ---------------------  
        [Display(Name = "Unidades")]
        public virtual ICollection<Unit> Units { get; set; } = new List<Unit>();

        // ----- 🔥  Campos de Auditoría -----------------------------------------------
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
