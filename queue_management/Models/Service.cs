using queue_management.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace queue_management.Models
{
    [Table("Services")]
    public class Service
    {
        // 🔥 -----   Propiedades de la Entidad -----------------
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id del Servicio")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int ServiceId { get; set; }

        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Nombre del Servicio")]
        [StringLength(100)]
        public string? ServiceName { get; set; } = null!;

        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [StringLength(255)]
        public string? Description { get; set; } = null!;

        [StringLength(100)]
        [Display(Name = "Tipo de Servicio")]
        public int ServiceType { get; set; }

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

        // 🔥 -----   Entidad City   ---------------------
        [ForeignKey("City")]
        [Display(Name = "Id de Ciudad")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int CityId { get; set; }

        [Display(Name = "Nombre de la Ciudad")]
        public virtual City City { get; set; } = null!;

        // 🔥 -----   Entidad Location   ---------------------
        [ForeignKey("Location")]
        [Display(Name = "Id de la Ubicación")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        public int LocationId { get; set; }

        [Display(Name = "Nombre de la Ubicación")]
        public virtual Location Location { get; set; } = null!;

        // 🔥 -----   Propiedades de la Organizativas    ----------
        [ForeignKey("Areas")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Área")]
        public int AreaId { get; set; }
        public virtual Area Area { get; set; } = null!;

        [ForeignKey("Unidades")]
        [Required(ErrorMessage = "El campo {0} es obligatorio")]
        [Display(Name = "Unidad")]

        public int UnitId { get; set; }
        [Display(Name = "Nombre de la Unidad")]
        public virtual Unit Unit { get; set; } = null!;


        // 🔥 -----   Propiedades de Navegacion   ---------------------
        public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public virtual ICollection<ServiceWindow> ServiceWindows { get; set; } = new List<ServiceWindow>();
        public virtual ICollection<Queue> Queues { get; set; }  = new List<Queue>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
        public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

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
