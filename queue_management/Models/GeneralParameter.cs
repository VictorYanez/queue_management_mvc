using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace queue_management.Models
{
    public class GeneralParameter
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [Display(Name = "Id del Parámetro")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Display(Name = "Nombre del Parámetro")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(150)]
        public string? KeyName { get; set; }

        [Display(Name = "Valor del Parámetro")]
        [StringLength(250)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public string? Value { get; set; }

    }
}
