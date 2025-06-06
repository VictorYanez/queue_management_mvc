using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
    public class CitiesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<CitiesController> _logger;

        // 📌 Inyección de dependencias para contexto y logger
        public CitiesController(ApplicationDBContext context, ILogger<CitiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ----- 🔥 ----- Métodos POST & GET del (CRUD) ----- 🔥 ----- //
        #region Index & Details Actions
        // GET: Cities
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // 🚀 Consulta optimizada con ThenInclude y AsNoTracking para solo lectura
                var cities = await _context.Cities
                    .Include(c => c.District)
                        .ThenInclude(d => d.Municipality)
                            .ThenInclude(m => m.Department)
                                .ThenInclude(d => d.Country)
                    .AsNoTracking()
                    .ToListAsync();

                _logger.LogInformation("Listado de ciudades cargado exitosamente");
                return View(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el listado de ciudades");
                return RedirectToAction(nameof(Error));
            }
        }

        // GET: Cities/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details solicitado sin ID");
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.District)
                    .ThenInclude(d => d.Municipality)
                        .ThenInclude(m => m.Department)
                            .ThenInclude(d => d.Country)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.CityId == id);

            if (city == null)
            {
                _logger.LogWarning($"Ciudad con ID {id} no encontrada");
                return NotFound();
            }

            return View(city);
        }
        #endregion

        #region Create Actions
        // GET: Cities/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateCityDropdowns();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para creación de ciudad");
                return RedirectToAction(nameof(Error));
            }
        }

        public IActionResult CreateCheck()
        {
            ViewBag.MunicipalityId = new SelectList(_context.Municipalities, "MunicipalityId", "MunicipalityName");
            return View();
        }

        // POST: Cities/Create
        [HttpPost]
        // 🔒 Protección contra CSRF
        [ValidateAntiForgeryToken] 
        public async Task<IActionResult> Create([Bind("CityId,Name,Code,DistrictId,IsActive")] City city)
        {
            try
            {
                // 🛡️ Validación de modelo con eliminación de campos no necesarios              
                ModelState.Remove(nameof(City.Country));
                ModelState.Remove(nameof(City.Department));
                ModelState.Remove(nameof(City.Municipality));
                ModelState.Remove(nameof(City.District));

                if (ModelState.IsValid)
                {
                    _context.Add(city);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Ciudad {city.CityName} creada exitosamente");
                    return RedirectToAction(nameof(Index));
                }

                await PopulateCityDropdowns(city.DistrictId);
                return View(city);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear ciudad {city?.CityName}");
                await PopulateCityDropdowns(city?.DistrictId);
                return View(city);
            }
        }
        #endregion


        #region Edit Actions
        // GET: Cities/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit GET solicitado sin ID");
                return NotFound();
            }

            var city = await _context.Cities.FindAsync(id);
            if (city == null)
            {
                _logger.LogWarning($"Edit GET - Ciudad con ID {id} no encontrada");
                return NotFound();
            }

            await PopulateCityDropdowns(city.DistrictId);
            return View(city);
        }

        // POST: Cities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        // 🔒 Protección contra CSRF
        public async Task<IActionResult> Edit(int id, [Bind("CityId,Name,Code,DistrictId,IsActive")] City city)
        {
            if (id != city.CityId)
            {
                _logger.LogWarning($"Edit POST - ID {id} no coincide con CityId {city.CityId}");
                return NotFound();
            }

            ModelState.Remove("District");
            ModelState.Remove("Locations");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(city);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Ciudad ID {id} actualizada exitosamente");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!CityExists(city.CityId))
                    {
                        _logger.LogWarning($"Edit POST - Ciudad con ID {city.CityId} no existe");
                        return NotFound();
                    }
                    _logger.LogError(ex, $"Error de concurrencia al editar ciudad ID {city.CityId}");
                    ModelState.AddModelError("", "Error de concurrencia. El registro fue modificado por otro usuario.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error general al editar ciudad ID {city.CityId}");
                    ModelState.AddModelError("", "Ocurrió un error al actualizar la ciudad.");
                }
            }

            await PopulateCityDropdowns(city.DistrictId);
            return View(city);
        }
        #endregion

        #region Delete Actions
        // GET: Cities/Delete
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var city = await _context.Cities
                .Include(c => c.District)
                .FirstOrDefaultAsync(m => m.CityId == id);

            if (city == null)
            {
                return NotFound();
            }

            return View(city);
        }

        // POST: Cities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        // 🔒 Protección contra CSRF
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var city = await _context.Cities.FindAsync(id);
            if (city != null)
            {
                // 📌 Verificación de referencias antes de eliminar
                var hasLocations = await _context.Locations.AnyAsync(l => l.CityId == id);
                if (hasLocations)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar la ciudad porque tiene localizaciones asociadas.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.Cities.Remove(city);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Ciudad ID {id} eliminada exitosamente");
            }

            return RedirectToAction(nameof(Index));
        }
        #endregion

        // -----  🔥  ----- Funciones Adicionales ---------  🔥  ------ 
        // 📌 Método para verificar existencia de la Ciudad ----------
        private bool CityExists(int id)
        {
            return _context.Cities.Any(e => e.CityId == id);
        }

        // 📌 Método para listas desplegables en vistas de Cities
        #region Dropdown Population Methods
        private async Task PopulateCityDropdowns(int? selectedDistrictId = null)
        {
            try
            {
                ViewData["Countries"] = new SelectList(
                    await _context.Countries
                        .OrderBy(c => c.CountryName)
                        .AsNoTracking()
                        .ToListAsync(),
                    "CountryId", "Name");

                if (selectedDistrictId.HasValue)
                {
                    var district = await _context.Districts
                        .Include(d => d.Municipality)
                            .ThenInclude(m => m.Department)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(d => d.DistrictId == selectedDistrictId);

                    if (district != null)
                    {
                        ViewData["Departments"] = new SelectList(
                            await _context.Departments
                                .Where(d => d.CountryId == district.Municipality.Department.CountryId)
                                .OrderBy(d => d.DepartmentName)
                                .AsNoTracking()
                                .ToListAsync(),
                            "DepartmentId", "Name", district.Municipality.DepartmentId);

                        ViewData["Municipalities"] = new SelectList(
                            await _context.Municipalities
                                .Where(m => m.DepartmentId == district.Municipality.DepartmentId)
                                .OrderBy(m => m.MunicipalityName)
                                .AsNoTracking()
                                .ToListAsync(),
                            "MunicipalityId", "Name", district.MunicipalityId);

                        ViewData["DistrictId"] = new SelectList(
                            await _context.Districts
                                .Where(d => d.MunicipalityId == district.MunicipalityId)
                                .OrderBy(d => d.DistrictName)
                                .AsNoTracking()
                                .ToListAsync(),
                            "DistrictId", "Name", selectedDistrictId);
                    }
                }
                else
                {
                    ViewData["Departments"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewData["Municipalities"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewData["DistrictId"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al poblar dropdowns para ciudad");
                throw;
            }
        }
        #endregion

        #region JSON Endpoints
        [HttpGet]
        public async Task<JsonResult> GetDistrictsByMunicipality(int municipalityId)
        {
            try
            {
                var districts = await _context.Districts
                    .Where(d => d.MunicipalityId == municipalityId)
                    .OrderBy(d => d.DistrictName)
                    .Select(d => new { d.DistrictId, d.DistrictName })
                    .AsNoTracking()
                    .ToListAsync();

                return Json(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener distritos para municipio ID {municipalityId}");
                return Json(new { error = "Error al cargar distritos" });
            }
        }

        [HttpGet]
        public async Task<JsonResult> GetCitiesByDistrict(int districtId)
        {
            try
            {
                var cities = await _context.Cities
                    .Where(c => c.DistrictId == districtId)
                    .OrderBy(c => c.CityName)
                    .Select(c => new { c.CityId, c.CityName })
                    .AsNoTracking()
                    .ToListAsync();

                return Json(cities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener ciudades para distrito ID {districtId}");
                return Json(new { error = "Error al cargar ciudades" });
            }
        }
        #endregion

        #region Helper Methods
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}