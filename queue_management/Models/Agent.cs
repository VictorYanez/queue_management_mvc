using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;

namespace queue_management.Models
{
    [Table("Agents")]
    public class Agent
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id del Agente")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int AgentId { get; set; }

        [Display(Name = "Documento de Identidad")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        [StringLength(20)]
        public string? DUI { get; set; }

        [Display(Name = "Nombres")]
        [StringLength(100, MinimumLength = 2)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? FirstName { get; set; }

        [Display(Name = "Apellidos")]
        [StringLength(100, MinimumLength = 2)]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? LastName { get; set; }

        [Display(Name = "Correo")]
        [EmailAddress]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? Email { get; set; }

        [Display(Name = "Teléfono")]
        [Phone]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public string? PhoneNumber { get; set; }

        // 🔥 Definición de Relaciones & Propiedad de Navegacion  
        [ForeignKey("Role")]
        [Display(Name = "Role Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int RoleId { get; set; }   //Puede tener varios roles ?

        // 🔥 -----   Entidad Departamento   ---------------------
        [ForeignKey("Department")]
        [Display(Name = "Id del Departamento")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int DepartmentId { get; set; }

        [Display(Name = "Nombre del Departamento")]
        public virtual Department Department { get; set; } = null!;

        // 🔥 -----   Entidad Region   ---------------------
        [ForeignKey("Region")]
        [Display(Name = "Id de la Región")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int RegionId { get; set; }

        [Display(Name = "Nombre de la Región")]
        public virtual Region Region { get; set; } = null!;

        // 🔥 -----   Entidad Municipality   ---------------------
        [ForeignKey("Municipality")]
        [Display(Name = "Id de Municipio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int MunicipalityId { get; set; }

        [Display(Name = "Nombre de Municipio")]
        public virtual Municipality Municipality { get; set; } = null!;

        // 🔥 -----   Entidad City   ---------------------
        [ForeignKey("City")]
        [Display(Name = "Id de Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int CityId { get; set; }

        [Display(Name = "Nombre de la Ciudad")]
        public virtual City City { get; set; } = null!;

        // 🔥 -----   Entidad Location   ---------------------
        [ForeignKey("Location")]
        [Display(Name = "Ubicación Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int LocationId { get; set; }

        [Display(Name = "Nombre de la Ubicación")]
        public Location? Location { get; set; }

        // 🔥 -----   Entidad Unidad   ---------------------
        [ForeignKey("Unit")]
        [Display(Name = "Ubicación Id")]
        [Required(ErrorMessage = "El campo {0} es requerido")]
        public int UnitId { get; set; }

        [Display(Name = "Nombre de la Unidad")]
        public string? Unit { get; set; }

        //--------  ?????
        [Display(Name = "Nombre de la Posición")]
        public string? Position { get; set; }

        // 🔥 -----   Propiedades de Navegacion   ---------------------
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
