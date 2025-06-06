
using queue_management.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Ratings")]
    public class Rating
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id de la Evaluación")]
        public int RatingId { get; set; }

        [Display(Name = "Fecha de la Evaluación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [CustomValidation(typeof(Rating), "ValidateEvaluationDate")]
        public DateTime EvaluationDate { get; set; }

        [Display(Name = "Nota de la Evaluación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Range(0, 10, ErrorMessage = "Los valores permitidos para {0} son entre {1} y {2}.")]
        public int? Score { get; set; }

        [Display(Name = "Observación de la Evaluación")]
        public string? Observations { get; set; }
        [MaxLength(250, ErrorMessage = "Las observaciones no pueden exceder los {1} caracteres.")]

        // 🔥 Definición de Relaciones & Propiedad de Navegacion  -----
        [ForeignKey("User")]
        [Display(Name = "Id del Usuario")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [CustomValidation(typeof(Rating), "ValidateUserIsActive")]
        public int UserId { get; set; }

        //[Display(Name = "Usuario")]
        //public virtual User? User { get; set; }

        // 🔥 Definición de Relaciones Servicio  -----
        [ForeignKey("Service")]
        [Display(Name = "Id del Servicio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ServiceId { get; set; }

        [Display(Name = "Servicio")]
        public virtual Service? Service { get; set; }

        // 🔥 Definición de Relaciones Location  -----
        [ForeignKey("Location")]
        [Display(Name = "Id de la Ubicación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int LocationId { get; set; }

        [Display(Name = "Ubicación")]
        public virtual Location? Location { get; set; }

        // 🔥 -----   Propiedades de Navegacion   ---------------------
        [Display(Name = "Region")]
        public virtual ICollection<Region> Regiones { get; set; } = new List<Region>();

        // 🔥 -----  Campos de Auditoría    ------------------------------------------
        // ------- Información de Datos en Log (Registro – Creación) -----------------
        [ForeignKey("CreatedByUser")]
        [ScaffoldColumn(false)]
        public int? CreatedBy { get; set; }
        // [ScaffoldColumn(false)]
        // public virtual User CreatedByUser { get; set; }  // Propiedad de navegacion
        [ScaffoldColumn(false)]
        public DateTime? CreatedAt { get; set; }

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

        // 🔥 -----  Métodos de Validación Personalizados -----------------
        public static ValidationResult ValidateEvaluationDate(DateTime evaluationDate, ValidationContext context)
        {
            if (evaluationDate > DateTime.Now)
            {
                return new ValidationResult("La fecha de evaluación no puede ser futura.");
            }
            return ValidationResult.Success!; // Usamos el operador ! para indicar que no es nulo
        }

    }
}
