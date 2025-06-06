using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using queue_management.Data;
using queue_management.Models;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace queue_management.Controllers
{
    [Authorize]
    public class MunicipalitiesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<MunicipalitiesController> _logger;

        public MunicipalitiesController(ApplicationDBContext context, ILogger<MunicipalitiesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // ----- 🔥 ----- Métodos Principales (CRUD) ----- 🔥 ----- //

        #region Index & Details Actions
        // GET: Municipalities
        public async Task<IActionResult> Index()
        {
            try
            {
                // 🚀 Consulta optimizada con ThenInclude y AsNoTracking
                var municipalities = await _context.Municipalities
                    .Include(m => m.Department)
                        .ThenInclude(d => d.Country)
                    .AsNoTracking()
                    .OrderBy(m => m.MunicipalityName)
                    .ToListAsync();

                _logger.LogInformation("Listado de municipios cargado exitosamente");
                return View(municipalities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar el listado de municipios");
                return RedirectToAction(nameof(Error));
            }
        }

        // GET: Municipalities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Details solicitado sin ID");
                return NotFound();
            }

            var municipality = await _context.Municipalities
                .Include(m => m.Department)
                    .ThenInclude(d => d.Country)
                .Include(m => m.Cities)
                .AsNoTracking()
                .FirstOrDefaultAsync(m => m.MunicipalityId == id);

            if (municipality == null)
            {
                _logger.LogWarning($"Municipio con ID {id} no encontrado");
                return NotFound();
            }

            return View(municipality);
        }
        #endregion

        #region Create Actions
        // GET: Municipalities/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            try
            {
                await PopulateMunicipalityDropdowns();
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar datos para creación de municipio");
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: Municipalities/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MunicipalityId,MunicipalityName,Code,CountryId,DepartmentId,IsActive")] Municipality municipality)
        {
            try
            {
                // 🛡️ Validación de modelo con eliminación de campos no necesarios
                ModelState.Remove(nameof(Municipality.Department));
                ModelState.Remove(nameof(Municipality.Country));
                ModelState.Remove(nameof(Municipality.Cities));

                if (ModelState.IsValid)
                {
                    // Set audit fields
                    municipality.CreatedAt = DateTime.UtcNow;
                    municipality.CreatedBy = GetCurrentUserId();

                    _context.Add(municipality);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Municipio {municipality.MunicipalityName} creado exitosamente");
                    return RedirectToAction(nameof(Index));
                }

                await PopulateMunicipalityDropdowns(municipality.CountryId);
                return View(municipality);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al crear municipio {municipality?.MunicipalityName}");
                await PopulateMunicipalityDropdowns(municipality?.CountryId);
                return View(municipality);
            }
        }
        #endregion

        #region Edit Actions
        // GET: Municipalities/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                _logger.LogWarning("Edit GET solicitado sin ID");
                return NotFound();
            }

            try
            {
                var municipality = await _context.Municipalities.FindAsync(id);
                if (municipality == null)
                {
                    _logger.LogWarning($"Edit GET - Municipio con ID {id} no encontrado");
                    return NotFound();
                }

                await PopulateMunicipalityDropdowns(municipality.DepartmentId);
                return View(municipality);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar formulario de edición para municipio ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: Municipalities/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MunicipalityId,MunicipalityName,Code,CountryId,DepartmentId,IsActive,RowVersion")] Municipality municipality)
        {
            if (id != municipality.MunicipalityId)
            {
                _logger.LogWarning($"Edit POST - ID {id} no coincide con MunicipalityId {municipality.MunicipalityId}");
                return NotFound();
            }

            // 🛡️ Validación de modelo con eliminación de campos no necesarios
            ModelState.Remove(nameof(Municipality.Department));
            ModelState.Remove(nameof(Municipality.Country));
            ModelState.Remove(nameof(Municipality.Cities));

            if (ModelState.IsValid)
            {
                try
                {
                    // Set audit fields
                    municipality.ModifiedAt = DateTime.UtcNow;
                    municipality.ModifiedBy = GetCurrentUserId();

                    _context.Update(municipality);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Municipio ID {id} actualizado exitosamente");
                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (!MunicipalityExists(municipality.MunicipalityId))
                    {
                        _logger.LogWarning($"Edit POST - Municipio con ID {municipality.MunicipalityId} no existe");
                        return NotFound();
                    }
                    _logger.LogError(ex, $"Error de concurrencia al editar municipio ID {municipality.MunicipalityId}");
                    ModelState.AddModelError("", "Error de concurrencia. El registro fue modificado por otro usuario.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error general al editar municipio ID {municipality.MunicipalityId}");
                    ModelState.AddModelError("", "Ocurrió un error al actualizar el municipio.");
                }
            }

            await PopulateMunicipalityDropdowns(municipality.CountryId);
            return View(municipality);
        }
        #endregion

        #region Delete Actions
        // GET: Municipalities/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                _logger.LogWarning("Delete GET solicitado sin ID o con ID inválido");
                return NotFound();
            }

            try
            {
                var municipality = await _context.Municipalities
                    .Include(m => m.Department)
                    .Include(m => m.Country)
                    .Include(m => m.Cities)
                    .FirstOrDefaultAsync(m => m.MunicipalityId == id);

                if (municipality == null)
                {
                    _logger.LogWarning($"Delete GET - Municipio con ID {id} no encontrado");
                    return NotFound();
                }

                var hasDistricts = await _context.Districts.AnyAsync(d => d.MunicipalityId == id);
                ViewBag.HasDependencies = hasDistricts;
                return View(municipality);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar formulario de eliminación para municipio ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: Municipalities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var municipality = await _context.Municipalities.FindAsync(id);

            if (municipality != null)
            {
                // 📌 Verificación de referencias antes de eliminar
                var hasDistricts = await _context.Districts.AnyAsync(d => d.MunicipalityId == id);
                if (hasDistricts)
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el municipio porque tiene distritos asociados.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.Municipalities.Remove(municipality);
                await _context.SaveChangesAsync();
                _logger.LogInformation($"Municipio ID {id} eliminado exitosamente");
                return RedirectToAction(nameof(Index));
            }

            _logger.LogWarning($"Delete POST - Municipio con ID {id} no encontrado");
            return NotFound();
                }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar municipio ID {id}");
                TempData["ErrorMessage"] = "Ocurrió un error al eliminar el municipio.";
                return RedirectToAction(nameof(Delete), new { id });
            }
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        #endregion

        // ----- 🔥 ----- Funciones Adicionales ----- 🔥 ----- //
        #region Dropdown Population Methods
        private async Task PopulateMunicipalityDropdowns(int? selectedDepartmentId = null)
        {
            try
            {
                // 1. Establecer país por defecto
                int defaultDepartmentId = await GetDefaultDepartmentId(1);

                // 2. Cargar departamentos del país por defecto
                var departments = await _context.Departments
                    .Where(d => d.CountryId == defaultDepartmentId)
                    .OrderBy(d => d.DepartmentName)
                    .AsNoTracking()
                    .ToListAsync();

                // 3. Si hay departmentId, validar que pertenezca al país
                if (selectedDepartmentId.HasValue)
                {
                    var isValid = departments.Any(d => d.DepartmentId == selectedDepartmentId);
                    if (!isValid) selectedDepartmentId = null;
                }

                // 4. Asignar ViewData
                ViewData["Countries"] = new SelectList(Enumerable.Empty<SelectListItem>());
                ViewData["Departments"] = new SelectList(departments, "DepartmentId", "DepartmentName", selectedDepartmentId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar dropdowns");
                ViewData["Departments"] = new SelectList(Enumerable.Empty<SelectListItem>());
            }
        }
        #endregion

        #region JSON Endpoints
        [HttpGet]
        public async Task<JsonResult> GetDepartmentsByCountry(int countryId)
        {
            try
            {
                var departments = await _context.Departments
                    .Where(d => d.CountryId == countryId)
                    .OrderBy(d => d.DepartmentName)
                    .Select(d => new { d.DepartmentId, d.DepartmentName })
                    .AsNoTracking()
                    .ToListAsync();

                return Json(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al obtener departamentos para país ID {countryId}");
                return Json(new { error = "Error al cargar departamentos" });
            }
        }
        #endregion

        #region Helper Methods
        private bool MunicipalityExists(int id)
        {
            return _context.Municipalities.Any(e => e.MunicipalityId == id);
        }

        private int GetCurrentUserId()
        {
            // Implementar según tu sistema de autenticación
            // Ejemplo simplificado:
            return int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "0");
        }

        // Por la jerarquia de las entidades, al conocer el departamento, se puede obtener el país
        private async Task<int> GetDefaultDepartmentId(int countryId)
        {
            return await _context.Departments
                .Where(d => d.CountryId == countryId && d.IsDefault)
                .OrderBy(d => d.DepartmentId)
                .Select(d => d.DepartmentId)
                .FirstOrDefaultAsync();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}