using System;
using System.Threading.Tasks;
using System.Linq;
using queue_management.Data;
using queue_management.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace queue_management.Services
{
    public class GeographyService : IGeographyService
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<GeographyService> _logger;

        public GeographyService(ApplicationDBContext context, ILogger<GeographyService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Default Selections Implementation
        // ----- 🔥 ----- Default Selections ----- 🔥 ----- //
        public async Task<(int CountryId, int DepartmentId, int MunicipalityId)> GetDefaultSelections()
        {
            try
            {
                // 1. Obtener país por defecto
                var defaultCountry = await _context.Countries
                    .FirstOrDefaultAsync(c => c.IsDefault);

                if (defaultCountry == null)
                    return (0, 0, 0);

                // 2. Obtener departamento por defecto para ese país
                var defaultDepartment = await _context.Departments
                    .FirstOrDefaultAsync(d => d.CountryId == defaultCountry.CountryId && d.IsDefault);

                // 3. Obtener municipio por defecto si existe departamento
                int defaultMunicipalityId = 0;
                if (defaultDepartment != null)
                {
                    var defaultMunicipality = await _context.Municipalities
                        .FirstOrDefaultAsync(m => m.DepartmentId == defaultDepartment.DepartmentId && m.IsDefault);

                    defaultMunicipalityId = defaultMunicipality?.MunicipalityId ?? 0;
                }

                return (
                    defaultCountry.CountryId,
                    defaultDepartment?.DepartmentId ?? 0,
                    defaultMunicipalityId
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo selecciones por defecto");
                throw;
            }
        }
        #endregion

        #region Set Default Methods
        // ----- 🔥 ----- Set Default Methods ----- 🔥 ----- //
        public async Task SetDefaultCountry(int countryId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 📌 1. Reset all country defaults
                await _context.Countries
                    .Where(c => c.IsDefault)
                    .ExecuteUpdateAsync(c => c.SetProperty(x => x.IsDefault, false));

                // 📌 2. Set new default
                await _context.Countries
                    .Where(c => c.CountryId == countryId)
                    .ExecuteUpdateAsync(c => c.SetProperty(x => x.IsDefault, true));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SetDefaultDepartment(int departmentId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 🛡️ 1. Obtener el departamento para validar el país
                var department = await _context.Departments
                    .Include(d => d.Country)
                    .FirstOrDefaultAsync(d => d.DepartmentId == departmentId);

                if (department == null)
                    throw new ArgumentException("Department not found");

                // 📌 2. Reset all department defaults for this country
                await _context.Departments
                    .Where(d => d.CountryId == department.CountryId && d.IsDefault)
                    .ExecuteUpdateAsync(d => d.SetProperty(x => x.IsDefault, false));

                // 📌 3. Set new default
                await _context.Departments
                    .Where(d => d.DepartmentId == departmentId)
                    .ExecuteUpdateAsync(d => d.SetProperty(x => x.IsDefault, true));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task SetDefaultMunicipality(int municipalityId)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // 🛡️ 1. Obtener el municipio para validar el departamento
                var municipality = await _context.Municipalities
                    .Include(m => m.Department)
                    .FirstOrDefaultAsync(m => m.MunicipalityId == municipalityId);

                if (municipality == null)
                    throw new ArgumentException("Municipality not found");

                // 📌 2. Reset all municipality defaults for this department
                await _context.Municipalities
                    .Where(m => m.DepartmentId == municipality.DepartmentId && m.IsDefault)
                    .ExecuteUpdateAsync(m => m.SetProperty(x => x.IsDefault, false));

                // 📌 3. Set new default
                await _context.Municipalities
                    .Where(m => m.MunicipalityId == municipalityId)
                    .ExecuteUpdateAsync(m => m.SetProperty(x => x.IsDefault, true));

                await transaction.CommitAsync();
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        #endregion
    }
}