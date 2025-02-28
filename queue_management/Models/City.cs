﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Cities")]
    public class City
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Required(ErrorMessage = "El campo id es obligatorio")]
        [Display(Name = "Id de la Ciudad")]
        public int CityID { get; set; }

        [StringLength(100)]
        [Required(ErrorMessage = "El campo Nombre de Ciudad es obligatorio")]
        [Display(Name = "Nombre de la Ciudad")]
        public string? CityName { get; set; }

        // Definición de Relaciones & propiedad de navegacion  
        // --------------------------------------------
        [ForeignKey("CountryID")]
        [Required(ErrorMessage = "El campo País es obligatorio")]
        [Display(Name = "Id del País")]
        public int CountryID { get; set; }

        [Display(Name = "Nombre del País")]
        public virtual Country Country { get; set; } = null!;

        [ForeignKey("DepartmentID")]
        [Required(ErrorMessage = "El campo Departamento es obligatorio")]
        [Display(Name = "Id del Departamento")]
        public int DepartmentID { get; set; }

        [Display(Name = "Nombre del Departamento")]
        public virtual Department Department { get; set; } = null!;

        [ForeignKey("RegionID")]
        [Required(ErrorMessage = "El campo Region es obligatorio")]
        [Display(Name = "Id de la Región")]
        public int RegionID { get; set; }

        [Display(Name = "Nombre de la Región")]
        public virtual Region Region { get; set; } = null!;

        [ForeignKey("MunicipalityID")]
        [Required(ErrorMessage = "El campo municipio es obligatorio")]
        [Display(Name = "Id de Municipio")]
        public int MunicipalityID { get; set; } 

        [Display(Name = "Nombre de Municipio")]
        public virtual Municipality Municipality { get; set; } = null!;

        [Display(Name = "Locales")]
        public ICollection<Location> Locations { get; set; } = new List<Location>();

        // Campos de Auditoría
        // --------------------------------------------
        [ForeignKey("CreatedByUser")]
        public int CreatedBy { get; set; }
        //public virtual User CreatedByUser { get; set; }  // Propiedad de navegacion
        public DateTime CreatedAt { get; set; }

        [ForeignKey("ModifiedByUser")]
        public int? ModifiedBy { get; set; }
        //public virtual User ModifiedByUser { get; set; }  // Propiedad de navegacion
        public DateTime? ModifiedAt { get; set; }

        [Timestamp] // Esto es para control de concurrencia en SQL Server
        public byte[]? RowVersion { get; set; }

    }
}
