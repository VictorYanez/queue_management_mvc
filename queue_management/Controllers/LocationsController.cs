using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using queue_management.Data;
using queue_management.Models;

namespace queue_management.Controllers
{
    [Authorize]
    public class LocationsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<LocationsController> _logger;

        public LocationsController(ApplicationDBContext context, ILogger<LocationsController> logger)
        {
            _context = context;
            _logger = logger;
        }


        #region Index & Details Action
        // GET: Locations
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                var locations = await _context.Locations
                    .AsNoTracking()
                    .Include(l => l.City)
                        .ThenInclude(c => c.District)
                            .ThenInclude(d => d.Municipality)
                                .ThenInclude(m => m.Department)
                                    .ThenInclude(d => d.Country)
                    .OrderBy(l => l.LocationName)
                    .ToListAsync();

                return View(locations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener la lista de ubicaciones");
                return RedirectToAction("Error", "Home");
            }
        }

        // GET: Locations/Details/
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Se intentó acceder a Details con ID nulo o inválido");
                return NotFound();
            }

            try
            {
                var location = await _context.Locations
                    .AsNoTracking()
                    .Include(l => l.City)
                        .ThenInclude(c => c.District)
                            .ThenInclude(d => d.Municipality)
                                .ThenInclude(m => m.Department)
                                    .ThenInclude(d => d.Country)
                    .FirstOrDefaultAsync(l => l.LocationId == id);

                if (location == null)
                {
                    _logger.LogWarning($"Ubicación con ID {id} no encontrada");
                    return NotFound();
                }

                return View(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalles de ubicación con ID {id}");
                return RedirectToAction("Error", "Home");
            }
        }
        #endregion

        #region Create Actions

        // GET: Locations/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateLocationDropdowns();
                return View(new Location());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de creación de ubicación");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Locations/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("LocationId,LocationName,PhoneNumber,CountryId,DepartmentId,MunicipalityId,DistrictId,CityId")]
            Location location)
        {
            try
            {
                // Remover propiedades de navegación para evitar validación innecesaria
                ModelState.Remove(nameof(Location.Country));
                ModelState.Remove(nameof(Location.Department));
                ModelState.Remove(nameof(Location.Municipality));
                ModelState.Remove(nameof(Location.District));
                ModelState.Remove(nameof(Location.City));

                if (ModelState.IsValid)
                {
                    location.CreatedAt = DateTime.UtcNow;
                    location.ModifiedAt = DateTime.UtcNow;

                    _context.Add(location);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Nueva ubicación creada con ID {location.LocationId}");
                    return RedirectToAction(nameof(Index));
                }

                await PopulateLocationDropdowns(location);
                return View(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear nueva ubicación");
                await PopulateLocationDropdowns(location);
                return View(location);
            }
        }
        #endregion

        #region Edit Actions
        // GET: Locations/Edit/
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Se intentó acceder a Edit con ID nulo o inválido");
                return NotFound();
            }

            try
            {
                var location = await _context.Locations
                    .AsNoTracking()
                    .FirstOrDefaultAsync(l => l.LocationId == id);

                if (location == null)
                {
                    _logger.LogWarning($"Ubicación con ID {id} no encontrada para edición");
                    return NotFound();
                }

                await PopulateLocationDropdowns(location);
                return View(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar formulario de edición para ubicación con ID {id}");
                return RedirectToAction("Error", "Home");
            }
        }


        // POST: Locations/Edit/
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("LocationId,LocationName,PhoneNumber,CountryId,DepartmentId,MunicipalityId,DistrictId,CityId,RowVersion")]
            Location location)
        {
            if (id != location.LocationId)
            {
                _logger.LogWarning($"ID de ubicación no coincide: {id} vs {location.LocationId}");
                return NotFound();
            }

            try
            {
                // Remover propiedades de navegación para evitar validación innecesaria
                ModelState.Remove(nameof(Location.Country));
                ModelState.Remove(nameof(Location.Department));
                ModelState.Remove(nameof(Location.Municipality));
                ModelState.Remove(nameof(Location.District));
                ModelState.Remove(nameof(Location.City));

                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingLocation = await _context.Locations.FindAsync(id);
                        if (existingLocation == null)
                        {
                            _logger.LogWarning($"Ubicación con ID {id} no encontrada para edición");
                            return NotFound();
                        }

                        // Actualizar solo los campos permitidos
                        existingLocation.LocationName = location.LocationName;
                        existingLocation.PhoneNumber = location.PhoneNumber;
                        existingLocation.CountryId = location.CountryId;
                        existingLocation.DepartmentId = location.DepartmentId;
                        existingLocation.MunicipalityId = location.MunicipalityId;
                        existingLocation.DistrictId = location.DistrictId;
                        existingLocation.CityId = location.CityId;
                        existingLocation.ModifiedAt = DateTime.UtcNow;

                        _context.Entry(existingLocation).OriginalValues["RowVersion"] = location.RowVersion;
                        _context.Update(existingLocation);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Ubicación con ID {id} actualizada exitosamente");
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        _logger.LogWarning(ex, $"Error de concurrencia al editar ubicación con ID {id}");
                        ModelState.AddModelError(string.Empty,
                            "El registro ha sido modificado por otro usuario. Por favor, refresque la página e intente nuevamente.");
                    }
                }

                await PopulateLocationDropdowns(location);
                return View(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al editar ubicación con ID {id}");
                await PopulateLocationDropdowns(location);
                return View(location);
            }
        }
        #endregion

        #region Delete Actions

        // GET: Locations/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Se intentó acceder a Delete con ID nulo o inválido");
                return NotFound();
            }

            try
            {
                var location = await _context.Locations
                    .AsNoTracking()
                    .Include(l => l.City)
                        .ThenInclude(c => c.District)
                            .ThenInclude(d => d.Municipality)
                                .ThenInclude(m => m.Department)
                                    .ThenInclude(d => d.Country)
                    .FirstOrDefaultAsync(l => l.LocationId == id);

                if (location == null)
                {
                    _logger.LogWarning($"Ubicación con ID {id} no encontrada para eliminación");
                    return NotFound();
                }

                return View(location);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar formulario de eliminación para ubicación con ID {id}");
                return RedirectToAction("Error", "Home");
            }
        }

        // POST: Locations/Delete
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var location = await _context.Locations.FindAsync(id);
                if (location == null)
                {
                    _logger.LogWarning($"Ubicación con ID {id} no encontrada para eliminación");
                    return NotFound();
                }

                _context.Locations.Remove(location);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Ubicación con ID {id} eliminada exitosamente");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar ubicación con ID {id}");
                return RedirectToAction("Error", "Home");
            }
        }
        #endregion


        #region Helper Methods
        // ----------  Métodos de auxiliares  ----------------- 
        private async Task PopulateLocationDropdowns(Location? location = null)
        {
            try
            {
                // Obtener todos los países
                var countries = await _context.Countries
                    .AsNoTracking()
                    .OrderBy(c => c.CountryName)
                    .ToListAsync();

                // Inicializar listas para otros niveles
                var departments = new List<Department>();
                var municipalities = new List<Municipality>();
                var districts = new List<District>();
                var cities = new List<City>();

                // Cargar datos dependientes si hay una ubicación con IDs válidos
                if (location != null)
                {
                    if (location.CountryId > 0)
                    {
                        departments = await _context.Departments
                            .AsNoTracking()
                            .Where(d => d.CountryId == location.CountryId)
                            .OrderBy(d => d.DepartmentName)
                            .ToListAsync();
                    }

                    if (location.DepartmentId > 0)
                    {
                        municipalities = await _context.Municipalities
                            .AsNoTracking()
                            .Where(m => m.DepartmentId == location.DepartmentId)
                            .OrderBy(m => m.MunicipalityName)
                            .ToListAsync();
                    }

                    if (location.MunicipalityId > 0)
                    {
                        districts = await _context.Districts
                            .AsNoTracking()
                            .Where(d => d.MunicipalityId == location.MunicipalityId)
                            .OrderBy(d => d.DistrictName)
                            .ToListAsync();
                    }

                    if (location.DistrictId > 0)
                    {
                        cities = await _context.Cities
                            .AsNoTracking()
                            .Where(c => c.DistrictId == location.DistrictId)
                            .OrderBy(c => c.CityName)
                            .ToListAsync();
                    }
                }

                // Crear SelectLists para cada nivel
                ViewData["CountryId"] = new SelectList(countries, "CountryId", "CountryName", location?.CountryId);
                ViewData["DepartmentId"] = new SelectList(departments, "DepartmentId", "DepartmentName", location?.DepartmentId);
                ViewData["MunicipalityId"] = new SelectList(municipalities, "MunicipalityId", "MunicipalityName", location?.MunicipalityId);
                ViewData["DistrictId"] = new SelectList(districts, "DistrictId", "DistrictName", location?.DistrictId);
                ViewData["CityId"] = new SelectList(cities, "CityId", "CityName", location?.CityId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para dropdowns geográficos");

                // Inicializar listas vacías en caso de error
                ViewData["CountryId"] = new SelectList(new List<Country>(), "CountryId", "CountryName");
                ViewData["DepartmentId"] = new SelectList(new List<Department>(), "DepartmentId", "DepartmentName");
                ViewData["MunicipalityId"] = new SelectList(new List<Municipality>(), "MunicipalityId", "MunicipalityName");
                ViewData["DistrictId"] = new SelectList(new List<District>(), "DistrictId", "DistrictName");
                ViewData["CityId"] = new SelectList(new List<City>(), "CityId", "CityName");
            }
        }

        #endregion

        #region JSON Endpoints for Cascading Dropdowns

        /// <summary>
        /// Obtiene los departamentos para un país específico (para dropdowns en cascada)
        /// </summary>
        /// <param name="countryId">ID del país</param>
        /// <returns>JSON con la lista de departamentos</returns>
        [HttpGet]
        public async Task<JsonResult> GetDepartments(int countryId)
        {
            if (countryId <= 0)
            {
                _logger.LogWarning($"Intento de obtener departamentos con countryId inválido: {countryId}");
                return Json(new { success = false, message = "ID de país inválido" });
            }

            try
            {
                var departments = await _context.Departments
                    .AsNoTracking()
                    .Where(d => d.CountryId == countryId)
                    .OrderBy(d => d.DepartmentName)
                    .Select(d => new { id = d.DepartmentId, name = d.DepartmentName })
                    .ToListAsync();

                return Json(new { success = true, data = departments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener departamentos para countryId {countryId}");
                return Json(new { success = false, message = "Error interno al obtener departamentos" });
            }
        }

        /// <summary>
        /// Obtiene los municipios para un departamento específico (para dropdowns en cascada)
        /// </summary>
        /// <param name="departmentId">ID del departamento</param>
        /// <returns>JSON con la lista de municipios</returns>
        [HttpGet]
        public async Task<JsonResult> GetMunicipalities(int departmentId)
        {
            if (departmentId <= 0)
            {
                _logger.LogWarning($"Intento de obtener municipios con departmentId inválido: {departmentId}");
                return Json(new { success = false, message = "ID de departamento inválido" });
            }

            try
            {
                var municipalities = await _context.Municipalities
                    .AsNoTracking()
                    .Where(m => m.DepartmentId == departmentId)
                    .OrderBy(m => m.MunicipalityName)
                    .Select(m => new { id = m.MunicipalityId, name = m.MunicipalityName })
                    .ToListAsync();

                return Json(new { success = true, data = municipalities });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener municipios para departmentId {departmentId}");
                return Json(new { success = false, message = "Error interno al obtener municipios" });
            }
        }

        /// <summary>
        /// Obtiene los distritos para un municipio específico (para dropdowns en cascada)
        /// </summary>
        /// <param name="municipalityId">ID del municipio</param>
        /// <returns>JSON con la lista de distritos</returns>
        [HttpGet]
        public async Task<JsonResult> GetDistricts(int municipalityId)
        {
            if (municipalityId <= 0)
            {
                _logger.LogWarning($"Intento de obtener distritos con municipalityId inválido: {municipalityId}");
                return Json(new { success = false, message = "ID de municipio inválido" });
            }

            try
            {
                var districts = await _context.Districts
                    .AsNoTracking()
                    .Where(d => d.MunicipalityId == municipalityId)
                    .OrderBy(d => d.DistrictName)
                    .Select(d => new { id = d.DistrictId, name = d.DistrictName })
                    .ToListAsync();

                return Json(new { success = true, data = districts });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener distritos para municipalityId {municipalityId}");
                return Json(new { success = false, message = "Error interno al obtener distritos" });
            }
        }

        /// <summary>
        /// Obtiene las ciudades para un distrito específico (para dropdowns en cascada)
        /// </summary>
        /// <param name="districtId">ID del distrito</param>
        /// <returns>JSON con la lista de ciudades</returns>
        [HttpGet]
        public async Task<JsonResult> GetCities(int districtId)
        {
            if (districtId <= 0)
            {
                _logger.LogWarning($"Intento de obtener ciudades con districtId inválido: {districtId}");
                return Json(new { success = false, message = "ID de distrito inválido" });
            }

            try
            {
                var cities = await _context.Cities
                    .AsNoTracking()
                    .Where(c => c.DistrictId == districtId)
                    .OrderBy(c => c.CityName)
                    .Select(c => new { id = c.CityId, name = c.CityName })
                    .ToListAsync();

                return Json(new { success = true, data = cities });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener ciudades para districtId {districtId}");
                return Json(new { success = false, message = "Error interno al obtener ciudades" });
            }
        }
        #endregion


    }
}

