using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace queue_management.Models
{
    [Table("Locations")]
    public class Location
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [Display(Name = "Id del Local")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LocationId { get; set; }

        [Display(Name = "Nombre del Local")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(200)]
        public string? LocationName { get; set; }

        [Display(Name = "Telefono del Local")]
        [StringLength(20)]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Phone]
        public string? PhoneNumber { get; set; }


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

        // 🔥 -----   Entidad Municipality   ---------------------
        [ForeignKey("Municipality")]
        [Display(Name = "Id de Municipio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int MunicipalityId { get; set; }

        [Display(Name = "Nombre de Municipio")]
        public virtual Municipality Municipality { get; set; } = null!;

        // 🔥 -----   Entidad District   ---------------------
        [ForeignKey("District")]
        [Display(Name = "Id del Distrito")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int DistrictId { get; set; }

        [Display(Name = "Nombre del Distrito")]
        public virtual District District { get; set; } = null!;

        // 🔥 -----  última Entidad de relación   ---------------------
        [ForeignKey("City")]
        [Display(Name = "Id de Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int CityId { get; set; } 

        [Display(Name = "Nombre de la Ciudad")]
        public virtual City City { get; set; } = null!;

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
