using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;
using queue_management.Data;
using queue_management.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace queue_management.Controllers
{
    [Authorize]
    public class DistrictsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<DistrictsController> _logger;

        public DistrictsController(ApplicationDBContext context, ILogger<DistrictsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ----- 🔥 ----- CRUD Actions ----- 🔥 ----- //
        #region Index & Details Actions
        //GET : Districts/Index
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            try
            {
                // 🚀 Consulta optimizada con ThenInclude y AsNoTracking para solo lectura
                var districts = await _context.Districts
                    .Include(d => d.Municipality)
                        .ThenInclude(m => m.Department)
                            .ThenInclude(d => d.Country)
                    .AsNoTracking()
                    .OrderBy(d => d.DistrictName)
                    .ToListAsync();

                _logger.LogInformation("Listado de distritos cargado exitosamente");
                return View(districts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el listado de distritos");
                return RedirectToAction(nameof(Error));
            }
        }

        //GET : Districts/Details
        [HttpGet]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Details solicitado sin ID o con ID inválido");
                return NotFound();
            }

            try
            {
                var district = await _context.Districts
                    .Include(d => d.Municipality)
                        .ThenInclude(m => m.Department)
                            .ThenInclude(d => d.Country)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(m => m.DistrictId == id);

                if (district == null)
                {
                    _logger.LogWarning($"Distrito con ID {id} no encontrado");
                    return NotFound();
                }

                return View(district);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener detalles del distrito ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }
        #endregion


        #region Create Actions
        // GET: Districts/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateDistrictDropdowns();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para creación de distrito");
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: Districts/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DistrictId,DistrictName,Code,MunicipalityId,IsActive")] District district)
        {
            try
            {
                // 🛡️ Validación de modelo con eliminación de campos no necesarios
                ModelState.Remove(nameof(District.Municipality));
                ModelState.Remove(nameof(District.Cities));

                if (ModelState.IsValid)
                {
                    _context.Add(district);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Distrito {district.DistrictName} creado exitosamente con ID {district.DistrictId}");
                    return RedirectToAction(nameof(Index));
                }

                await PopulateDistrictDropdowns(district.MunicipalityId);
                return View(district);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear distrito {district?.DistrictName}");
                await PopulateDistrictDropdowns(district?.MunicipalityId);
                return View(district);
            }
        }
        #endregion

        #region Edit Actions
        // GET: Districts/Delete/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Edit GET solicitado sin ID o con ID inválido");
                return NotFound();
            }

            try
            {
                var district = await _context.Districts.FindAsync(id);
                if (district == null)
                {
                    _logger.LogWarning($"Edit GET - Distrito con ID {id} no encontrado");
                    return NotFound();
                }

                await PopulateDistrictDropdowns(district.MunicipalityId);
                return View(district);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar formulario de edición para distrito ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: Districts/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DistrictId,DistrictName,Code,MunicipalityId,Status,RowVersion")] District district)
        {
            if (id != district.DistrictId)
            {
                _logger.LogWarning($"Edit POST - ID {id} no coincide con DistrictId {district.DistrictId}");
                return NotFound();
            }

            try
            {
                // 🛡️ Validación de modelo con eliminación de campos no necesarios
                ModelState.Remove(nameof(District.Municipality));
                ModelState.Remove(nameof(District.Cities));

                if (ModelState.IsValid)
                {
                    try
                    {
                        var existingDistrict = await _context.Districts.FindAsync(id);
                        if (existingDistrict == null)
                        {
                            _logger.LogWarning($"Edit POST - Distrito con ID {id} no encontrado");
                            return NotFound();
                        }

                        // Actualizar solo los campos permitidos
                        existingDistrict.DistrictName = district.DistrictName;
                        existingDistrict.VisibilityStatus = district.VisibilityStatus;
                        existingDistrict.MunicipalityId = district.MunicipalityId;

                        _context.Entry(existingDistrict).OriginalValues["RowVersion"] = district.RowVersion;
                        _context.Update(existingDistrict);
                        await _context.SaveChangesAsync();

                        _logger.LogInformation($"Distrito ID {id} actualizado exitosamente");
                        return RedirectToAction(nameof(Index));
                    }
                    catch (DbUpdateConcurrencyException ex)
                    {
                        if (!DistrictExists(district.DistrictId))
                        {
                            _logger.LogWarning($"Edit POST - Distrito con ID {district.DistrictId} no existe");
                            return NotFound();
                        }
                        _logger.LogError(ex, $"Error de concurrencia al editar distrito ID {district.DistrictId}");
                        ModelState.AddModelError("", "Error de concurrencia. El registro fue modificado por otro usuario.");
                    }
                }

                await PopulateDistrictDropdowns(district.MunicipalityId);
                return View(district);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error general al editar distrito ID {id}");
                await PopulateDistrictDropdowns(district.MunicipalityId);
                return View(district);
            }
        }
        #endregion

        // POST: Districts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var district = await _context.Districts.FindAsync(id);
            if (district == null)
            {
                return NotFound();
            }

            try
            {
                _context.Districts.Remove(district);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Error al eliminar distrito ID {id}");
                ModelState.AddModelError("", "No se puede eliminar, tiene datos relacionados");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        #region Helper Methods
        // ----- 🔥 ----- Helper Methods ----- 🔥 ----- //
        private bool DistrictExists(int id)
        {
            return _context.Districts.AsNoTracking().Any(e => e.DistrictId == id);
        }


        /// <param name="selectedMunicipalityId">ID del municipio seleccionado (opcional)</param>
        private async Task PopulateDistrictDropdowns(int? selectedMunicipalityId = null)
        {
            try
            {
                // Obtener todos los países
                ViewData["Countries"] = new SelectList(
                    await _context.Countries
                        .OrderBy(c => c.CountryName)
                        .AsNoTracking()
                        .ToListAsync(),
                    "CountryId", "CountryName");

                // Cargar datos dependientes si hay un municipio seleccionado
                if (selectedMunicipalityId.HasValue && selectedMunicipalityId > 0)
                {
                    var municipality = await _context.Municipalities
                        .Include(m => m.Department)
                        .AsNoTracking()
                        .FirstOrDefaultAsync(m => m.MunicipalityId == selectedMunicipalityId);

                    if (municipality != null)
                    {
                        // Departamentos del país seleccionado
                        ViewData["Departments"] = new SelectList(
                            await _context.Departments
                                .Where(d => d.CountryId == municipality.Department.CountryId)
                                .OrderBy(d => d.DepartmentName)
                                .AsNoTracking()
                                .ToListAsync(),
                            "DepartmentId", "DepartmentName", municipality.DepartmentId);

                        // Municipios del departamento seleccionado
                        ViewData["MunicipalityId"] = new SelectList(
                            await _context.Municipalities
                                .Where(m => m.DepartmentId == municipality.DepartmentId)
                                .OrderBy(m => m.MunicipalityName)
                                .AsNoTracking()
                                .ToListAsync(),
                            "MunicipalityId", "MunicipalityName", selectedMunicipalityId);
                    }
                }
                else
                {
                    // Inicializar listas vacías si no hay selección
                    ViewData["Departments"] = new SelectList(Enumerable.Empty<SelectListItem>());
                    ViewData["MunicipalityId"] = new SelectList(Enumerable.Empty<SelectListItem>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al poblar dropdowns para distrito");
                // Inicializar listas vacías en caso de error
                ViewData["Countries"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["Departments"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["MunicipalityId"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion

        #region JSON Endpoints
        // ----- 🔥 ----- JSON Endpoints for Cascading Dropdowns ----- 🔥 ----- //
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
                    .Where(d => d.CountryId == countryId)
                    .OrderBy(d => d.DepartmentName)
                    .Select(d => new { id = d.DepartmentId, name = d.DepartmentName })
                    .AsNoTracking()
                    .ToListAsync();

                return Json(new { success = true, data = departments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener departamentos para countryId {countryId}");
                return Json(new { success = false, message = "Error interno al obtener departamentos" });
            }
        }

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
                    .Where(m => m.DepartmentId == departmentId)
                    .OrderBy(m => m.MunicipalityName)
                    .Select(m => new { id = m.MunicipalityId, name = m.MunicipalityName })
                    .AsNoTracking()
                    .ToListAsync();

                return Json(new { success = true, data = municipalities });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener municipios para departmentId {departmentId}");
                return Json(new { success = false, message = "Error interno al obtener municipios" });
            }
        }
        #endregion

    }
}
