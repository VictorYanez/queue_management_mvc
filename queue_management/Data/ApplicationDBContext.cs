using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using queue_management.Models;

namespace queue_management.Data
{
    public class ApplicationDBContext : DbContext

    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        #region Definición de DbSets (Tablas)
        // Definición de Modelos del Sistema
        public DbSet<Agent> Agents { get; set; } = default!;
        public DbSet<Appointment> Appointments { get; set; } = default!;
        public DbSet<Area> Areas { get; set; } = default!;
        public DbSet<City> Cities { get; set; } = default!;
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<Country> Countries { get; set; } = default!;
        public DbSet<Department> Departments { get; set; } = default!;
        // Se adiciona el modelo de Distrito y se elimina el de Región
        public DbSet<District> Districts { get; set; } = default!;
        public DbSet<Location> Locations { get; set; } = default!;
        public DbSet<Municipality> Municipalities { get; set; } = default!;
        public DbSet<Queue> Queues { get; set; } = default!;
        public DbSet<QueueAssignment> QueueAssignments { get; set; } = default!;
        public DbSet<QueueStatus> QueueStatus { get; set; } = default!;
        public DbSet<QueueStatusAssignment> QueueStatusAssignments { get; set; } = default!;
        public DbSet<Rating> Ratings { get; set; } = default!;
        public DbSet<Role> Roles { get; set; } = default!;
        public DbSet<Service> Services { get; set; } = default!;
        public DbSet<ServiceStatus> ServiceStatuses { get; set; } = default!;
        public DbSet<ServiceWindow> ServiceWindows { get; set; } = default!;
        public DbSet<Status> Status { get; set; } = default!;
        public DbSet<Ticket> Tickets { get; set; } = default!;
        public DbSet<TicketStatus> TicketStatus { get; set; } = default!;
        public DbSet<TicketStatusAssignment> TicketStatusAssignments { get; set; } = default!;
        public DbSet<Unit> Units { get; set; } = default!;
        //public DbSet<User> Users { get; set; } = default!;
        #endregion

        // Resolución de relaciones de Identity
        // API Fluente
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            #region Eliminación en cascada para todas las relaciones
            // Deshabilitar eliminación en cascada por defecto para todas las relaciones
            foreach (var relationship in modelBuilder.Model.GetEntityTypes()
                .SelectMany(e => e.GetForeignKeys()))
            {
                relationship.DeleteBehavior = DeleteBehavior.Restrict;
            }
            #endregion

            #region Configuración Global .NET 8 (más estricta con tipos nullables)
            // Configuración para .NET 8 (más estricta con tipos nullables)
            foreach (var entityType in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if (property.ClrType == typeof(string))
                    {
                        property.IsNullable = false; // Todos los strings requeridos por defecto
                    }
                }
            }
            #endregion

            #region Configuración de Tablas
            // Asignación de nombres de tablas (pluralización)
            modelBuilder.Entity<Agent>().ToTable("Agents");
            modelBuilder.Entity<Appointment>().ToTable("Appointments");
            modelBuilder.Entity<Area>().ToTable("Areas");
            modelBuilder.Entity<City>().ToTable("Cities");
            modelBuilder.Entity<Comment>().ToTable("Comments");
            modelBuilder.Entity<Country>().ToTable("Countries");
            modelBuilder.Entity<Department>().ToTable("Departments");
            modelBuilder.Entity<District>().ToTable("Districts");
            modelBuilder.Entity<Location>().ToTable("Locations");
            modelBuilder.Entity<Municipality>().ToTable("Municipalities");
            modelBuilder.Entity<Queue>().ToTable("Queues");
            modelBuilder.Entity<QueueAssignment>().ToTable("QueueAssignments");
            modelBuilder.Entity<QueueStatus>().ToTable("QueueStatus");
            modelBuilder.Entity<QueueStatusAssignment>().ToTable("QueueStatusAssignments");
            modelBuilder.Entity<Rating>().ToTable("Ratings");
            modelBuilder.Entity<Role>().ToTable("Roles");
            modelBuilder.Entity<Service>().ToTable("Services");
            modelBuilder.Entity<ServiceWindow>().ToTable("ServiceWindows");
            modelBuilder.Entity<Status>().ToTable("Status");
            modelBuilder.Entity<Ticket>().ToTable("Tickets");
            modelBuilder.Entity<TicketStatus>().ToTable("TicketStatus");
            modelBuilder.Entity<TicketStatusAssignment>().ToTable("TicketStatusAssignments");
            modelBuilder.Entity<Unit>().ToTable("Units");
            #endregion

            #region Configuración de Relaciones
            // Relaciones definidas para las entidades desde el lado Principal
            // Configuración de Municipality con todas sus relaciones
            modelBuilder.Entity<Municipality>(entity =>
            {
                // Relación con Department (muchos municipalities pertenecen a un department)
                entity.HasOne(m => m.Department)
                    .WithMany(d => d.Municipalities)
                    .HasForeignKey(m => m.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);

                // Relación con Cities (un municipality tiene muchas cities)
                entity.HasMany(m => m.Cities)
                    .WithOne(c => c.Municipality)
                    .HasForeignKey(c => c.MunicipalityId)
                    .OnDelete(DeleteBehavior.Restrict);

            });


            // Relaciones definidas para las entidades desde el lado Dependiente
            // Relación Agent -> Appointment
            modelBuilder.Entity<Appointment>()
                .HasOne(ap => ap.Agent)
                .WithMany(ag => ag.Appointments)
                .HasForeignKey(ap => ap.AgentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Appointment -> Service
            modelBuilder.Entity<Appointment>()
                .HasOne(ap => ap.Service)
                .WithMany(se => se.Appointments)
                .HasForeignKey(ap => ap.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación Area - Unit
            modelBuilder.Entity<Unit>()
                .HasOne(u => u.Area)
                .WithMany(a => a.Units)
                .HasForeignKey(u => u.AreaId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Comment -> Ticket
            modelBuilder.Entity<Comment>()
                .HasOne(cm => cm.Ticket)
                .WithMany(ti => ti.Comments)
                .HasForeignKey(cm => cm.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Country -> Department
            modelBuilder.Entity<Department>()
                .HasOne(de => de.Country)
                .WithMany(co => co.Departments)
                .HasForeignKey(de => de.CountryId)
                .OnDelete(DeleteBehavior.Restrict);

            // Ejemplo Uno a Muchos: District -> Cities
            modelBuilder.Entity<District>()
                .HasMany(ci => ci.Cities)
                .WithOne(c => c.District)
                .HasForeignKey(c => c.DistrictId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Location -> Service
            modelBuilder.Entity<Service>()
                .HasOne(se => se.Location)
                .WithMany(lo => lo.Services)
                .HasForeignKey(se => se.LocationId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Queue -> QueueAssignment
            modelBuilder.Entity<QueueAssignment>()
                .HasOne(qa => qa.Queue)
                .WithMany(qu => qu.QueueAssignments)
                .HasForeignKey(qa => qa.QueueId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Queue -> Ticket
            modelBuilder.Entity<Queue>()
                .HasMany(qu => qu.Tickets)
                .WithOne(ti => ti.Queue)
                .HasForeignKey(ti => ti.QueueId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación QueueStatus -> QueueStatusAssignment
            modelBuilder.Entity<QueueStatusAssignment>()
                .HasOne(qsa => qsa.QueueStatus)
                .WithMany(qs => qs.QueueStatusAssignments)
                .HasForeignKey(qsa => qsa.QueueStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Service -> Rating
            modelBuilder.Entity<Rating>()
                .HasOne(ra => ra.Service)
                .WithMany(se => se.Ratings)
                .HasForeignKey(ra => ra.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación City - Country
            modelBuilder.Entity<City>()
                .HasOne(ci => ci.Country)
                .WithMany()
                .HasForeignKey(ci => ci.CountryId)
                .OnDelete(DeleteBehavior.Restrict); // Restringe la eliminación en cascada

            // Configuración de la relación City - Department
            modelBuilder.Entity<City>()
                .HasOne(c => c.Department)
                .WithMany()
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict); // Restringe la eliminación en cascada

            // Relación Country -> Location  
            modelBuilder.Entity<Location>()
                  .HasOne(l => l.Country)
                  .WithMany()
                  .HasForeignKey(l => l.CountryId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relación Department -> Location  
            modelBuilder.Entity<Location>()
                  .HasOne(l => l.Department)
                  .WithMany()
                  .HasForeignKey(l => l.DepartmentId)
                  .OnDelete(DeleteBehavior.Restrict);

            // Relación Municipality -> Location  
            modelBuilder.Entity<Location>()
                .HasOne(l => l.Municipality)
                .WithMany()
                .HasForeignKey(l => l.MunicipalityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configuración de la relación City - Municipality
            modelBuilder.Entity<City>()
                .HasOne(c => c.Municipality)
                .WithMany(m => m.Cities)
                .HasForeignKey(c => c.MunicipalityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación City -> Location
            modelBuilder.Entity<Location>()
                .HasOne(lo => lo.City)
                .WithMany(ci => ci.Locations)
                .HasForeignKey(lo => lo.CityId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Service -> ServiceWindow
            modelBuilder.Entity<ServiceWindow>()
                .HasOne(sw => sw.Service)
                .WithMany(se => se.ServiceWindows)
                .HasForeignKey(sw => sw.ServiceId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Ticket -> TicketStatusAssignment
            modelBuilder.Entity<TicketStatusAssignment>()
                .HasOne(tsa => tsa.Ticket)
                .WithMany(ti => ti.TicketStatusAssignments)
                .HasForeignKey(tsa => tsa.TicketId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación TicketStatus -> TicketStatusAssignment
            modelBuilder.Entity<TicketStatusAssignment>()
                .HasOne(tsa => tsa.TicketStatus)
                .WithMany(ts => ts.TicketStatusAssignments)
                .HasForeignKey(tsa => tsa.TicketStatusId)
                .OnDelete(DeleteBehavior.Restrict);

            // Relación Municipality -> Services
            modelBuilder.Entity<Service>()
                .HasOne(s => s.Municipality)
                .WithMany() // O .WithMany(m => m.Services) si tienes esa navegación
                .HasForeignKey(s => s.MunicipalityId)
                .OnDelete(DeleteBehavior.Restrict); // Asegúrate que esto esté configurado
            #endregion

            #region Configuración de Llaves Primarias
            // Configuración de autoincremento para IDs
            modelBuilder.Entity<Agent>()
                .Property(ag => ag.AgentId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Appointment>()
                .Property(ap => ap.AppointmentId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<City>()
                .Property(ci => ci.CityId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Comment>()
                .Property(cm => cm.CommentId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Country>()
                .Property(co => co.CountryId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Department>()
                .Property(d => d.DepartmentId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Location>()
                .Property(lo => lo.LocationId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Municipality>()
                .Property(mu => mu.MunicipalityId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Queue>()
                .Property(qu => qu.QueueId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<QueueStatus>()
                .Property(qs => qs.QueueStatusId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<QueueStatusAssignment>()
                .Property(qsa => qsa.QueueStatusAssignmentId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Rating>()
                .Property(ra => ra.RatingId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Role>()
                .Property(ro => ro.RoleId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Service>()
                .Property(se => se.ServiceId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<ServiceWindow>()
                .Property(sw => sw.WindowId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Status>()
                .Property(st => st.StatusId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Ticket>()
                .Property(ti => ti.TicketId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TicketStatus>()
                .Property(ts => ts.TicketStatusId)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<TicketStatusAssignment>()
                .Property(tsa => tsa.TicketStatusAssignmentId)
                .ValueGeneratedOnAdd();
            #endregion
        }
    }
}

