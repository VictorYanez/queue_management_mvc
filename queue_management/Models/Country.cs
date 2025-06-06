using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using queue_management.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using queue_management.Data;

namespace queue_management.Models
{
    [Table("Countries")]
    //[Authorize(Policy = "GeographyModuleAccess")] // 🔒 Seguridad: Control de acceso
    public class Country
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [Display(Name = "Id del País")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [ScaffoldColumn(false)]
        public int CountryId { get; set; } 

        [Display(Name = "Nombre del País")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(100, ErrorMessage = "El {0} debe tener máximo {1} caracteres")]
        public string CountryName { get; set; } = null!;

        [Display(Name = "Código del País")]
        [StringLength(14, ErrorMessage = "El {0} debe tener máximo {1} caracteres")]
        [Remote(action: "VerifyCustomCode", controller: "Countries", ErrorMessage = "Este código ya está en uso",
                AdditionalFields = nameof(CountryId))]
        public string CustomCode { get; set; } = string.Empty;

        // 🔥 -----  Estados de la Entidad    ----------
        [Display(Name = "Estado del País")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public VisibilityStatus VisibilityStatus { get; set; } = VisibilityStatus.Activo;

        [Display(Name = "País por Defecto")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public bool IsDefault { get; set; }

        // 🔥 --------  Propiedades de Navegación --------- 🔥 -----
        [Display(Name = "Departamentos")]
        [InverseProperty(nameof(Department.Country))]
        public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

        // ----- 🔥  Campos de Auditoría -----------------------------------------------
        // ------- Información de Datos en Log (Registro – Creación) -----------------
        [ForeignKey("CreatedByUser")]
        [ScaffoldColumn(false)]
        public int CreatedBy { get; set; }

        [ScaffoldColumn(false)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //[ForeignKey(nameof(ModifiedByUser))]
        //[ScaffoldColumn(false)]
        //public int? ModifiedBy { get; set; }

        [ScaffoldColumn(false)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd HH:mm:ss}", ApplyFormatInEditMode = true)]
        public DateTime? ModifiedAt { get; set; }

        // ------- Esto es para control de concurrencia en SQL Server -----------------
        [Timestamp]  
        public byte[]? RowVersion { get; set; }

        // 🔥 -----  Métodos de Validación Personalizados ----- 🔥 -----
        /// <summary>
        /// Valida que el nombre del país sea único
        /// </summary>
        /// <param name="countryName">Nombre del país a validar</param>
        /// <param name="context">Contexto de validación</param>
        /// <returns>Resultado de la validación</returns>
        public static ValidationResult? ValidateCountryName(string countryName, ValidationContext context)
        {
            // Validación de parámetros de entrada
            if (context.ObjectInstance is not Country instance)
            {
                throw new ArgumentException("El objeto de validación no es de tipo Country", nameof(context));
            }

            var dbContext = context.GetService(typeof(ApplicationDBContext)) as ApplicationDBContext;
            if (dbContext == null)
            {
                throw new InvalidOperationException("No se pudo obtener el DBContext para la validación");
            }

            // Validación principal
            if (dbContext.Countries.Any(c =>
                c.CountryName == countryName &&
                c.CountryId != instance.CountryId))
            {
                return new ValidationResult("El nombre del país ya existe", new[] { nameof(CountryName) });
            }

            return ValidationResult.Success;
        }

        /// <summary>
        /// Valida que el código personalizado sea único si se proporciona
        /// </summary>
        /// <param name="customCode">Código personalizado a validar</param>
        /// <param name="context">Contexto de validación</param>
        /// <returns>Resultado de la validación</returns>
        public static ValidationResult? ValidateCustomCode(string? customCode, ValidationContext context)
        {
            // Si el código es nulo o vacío, la validación es exitosa
            if (string.IsNullOrWhiteSpace(customCode))
                return ValidationResult.Success;

            // Validación de parámetros de entrada
            if (context.ObjectInstance is not Country instance)
            {
                throw new ArgumentException("El objeto de validación no es de tipo Country", nameof(context));
            }

            var dbContext = context.GetService(typeof(ApplicationDBContext)) as ApplicationDBContext;
            if (dbContext == null)
            {
                throw new InvalidOperationException("No se pudo obtener el DBContext para la validación");
            }

            // Validación principal
            if (dbContext.Countries.Any(c =>
                c.CustomCode == customCode &&
                c.CountryId != instance.CountryId))
            {
                return new ValidationResult("El código del país ya está en uso", new[] { nameof(CustomCode) });
            }

            return ValidationResult.Success;
        }
    }
}
