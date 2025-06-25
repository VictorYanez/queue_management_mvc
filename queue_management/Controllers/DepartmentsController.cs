using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using queue_management.Data;
using queue_management.Enums;
using queue_management.Models;
using queue_management.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace queue_management.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<DepartmentsController> _logger;
        private readonly IGeographyService _geoService;

        public DepartmentsController(ApplicationDBContext context, ILogger<DepartmentsController> logger, IGeographyService geoService)
        {
            _context = context;
            _logger = logger;
            _geoService = geoService;
        }


        #region CRUD Operations
        //Endpoint para la visualización del Listado 
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var departments = await _context.Departments
                    .Include(d => d.Country) // Carga relacionada de país
                    .Where(d => d.VisibilityStatus == VisibilityStatus.Activo)
                    .OrderBy(d => d.DepartmentName)
                    .AsNoTracking()
                    .ToListAsync();

                return View(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar departamentos");
                return RedirectToAction(nameof(Error));
            }
        }

        // GET: Departments/Details/5
        //Endpoint para la visualización de Detalles del registro  
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id <= 0)
                return NotFound();

            try
            {
                var department = await _context.Departments
                    .Include(d => d.Country)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DepartmentId == id);

                if (department == null)
                    return NotFound();

                ViewData["StatusOptions"] = GetStatusOptions();
                ViewData["CountryList"] = await GetCountriesSelectListAsync(department.CountryId);

                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar departamento ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }
        #endregion

        #region Create Operations
        // GET: Departments/Create
        [HttpGet("Create")]
        public async Task<IActionResult> Create()
        {
            try
            {
                ViewData["StatusOptions"] = GetStatusOptions();
                ViewData["CountryList"] = await GetCountriesSelectListAsync();

                return View(new Department
                {
                    VisibilityStatus = VisibilityStatus.Activo,
                    IsDefault = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar formulario de creación");
                return RedirectToAction(nameof(Error));
            }
        }

        //Endpoint para la creación del Listado 
        // POST: Departments/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Create([Bind("DepartmentName,CustomCode,VisibilityStatus,IsDefault,CountryId")] Department department)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Validar código único
                    if (await CustomCodeExists(department.CustomCode))
                    {
                        ModelState.AddModelError("CustomCode", "El código ya está en uso");
                        ViewData["StatusOptions"] = GetStatusOptions();
                        ViewData["CountryList"] = await GetCountriesSelectListAsync(department.CountryId);
                        return View(department);
                    }

                    // Manejar departamento por defecto
                    if (department.IsDefault)
                        await _geoService.SetDefaultDepartment(department.DepartmentId);

                    // Auditoría
                    department.CreatedAt = DateTime.UtcNow;
                    //department.CreatedBy = int.Parse(User.FindFirst("UserId").Value);

                    _context.Add(department);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Departamento creado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["StatusOptions"] = GetStatusOptions();
                ViewData["CountryList"] = await GetCountriesSelectListAsync(department.CountryId);
                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear departamento");
                ModelState.AddModelError("", "Error al crear departamento");
                return RedirectToAction(nameof(Error));
            }
        }
        #endregion

        #region Edit Operations

        // GET: Departments/Edit/5
        [HttpGet("Edit/{id:int}")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id <= 0)
                return NotFound();

            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                    return NotFound();

                ViewData["StatusOptions"] = GetStatusOptions();
                ViewData["CountryList"] = await GetCountriesSelectListAsync(department.CountryId);

                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar departamento ID {id} para edición");
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: Departments/Edit/5
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> Edit(int id, [Bind("DepartmentId,DepartmentName,CustomCode,VisibilityStatus,IsDefault,CountryId,RowVersion")] Department department)
        {
            if (id != department.DepartmentId)
                return NotFound();

            try
            {
                if (ModelState.IsValid)
                {
                    // Validar código único
                    if (await CustomCodeExists(department.CustomCode, department.DepartmentId))
                    {
                        ModelState.AddModelError("CustomCode", "El código ya está en uso");
                        ViewData["StatusOptions"] = GetStatusOptions();
                        ViewData["CountryList"] = await GetCountriesSelectListAsync(department.CountryId);
                        return View(department);
                    }

                    // Manejar departamento por defecto
                    if (department.IsDefault)
                        await _geoService.SetDefaultDepartment(department.DepartmentId);

                    // Auditoría
                    department.ModifiedAt = DateTime.UtcNow;
                    department.ModifiedBy = int.Parse(User.FindFirst("UserId").Value);

                    _context.Update(department);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Departamento actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["StatusOptions"] = GetStatusOptions();
                ViewData["CountryList"] = await GetCountriesSelectListAsync(department.CountryId);
                return View(department);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DepartmentExists(id))
                    return NotFound();

                _logger.LogError(ex, $"Error de concurrencia al editar departamento ID {id}");
                ModelState.AddModelError("", "El registro fue modificado por otro usuario. Por favor refresque y vuelva a intentar.");
                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al editar departamento ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }

        #endregion

        #region Delete Operations

        // GET: Departments/Delete/5
        [HttpGet("Delete/{id:int}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id <= 0)
                return NotFound();

            try
            {
                var department = await _context.Departments
                    .Include(d => d.Country)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(d => d.DepartmentId == id);

                if (department == null)
                    return NotFound();

                // Verificar dependencias
                if (await HasDependencies(id.Value))
                {
                    ViewBag.ErrorMessage = "No se puede eliminar porque tiene municipios o unidades asociadas";
                    return View(department);
                }

                return View(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar departamento ID {id} para eliminación");
                return RedirectToAction(nameof(Error));
            }

        }



        // POST: Departments/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null)
                {
                    return NotFound();
                }

                // Verifica solo dependencias geográficas (municipios)
                if (await HasDependencies(id))
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el departamento porque tiene municipios asociados.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Departamento eliminado exitosamente";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar departamento ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }
        #endregion


        // POST: Funciones Adicionales 
        #region Helper Methods
        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.DepartmentId == id);
        }
        private async Task<bool> HasDependencies(int departmentId)
        {
            /// Verifica si existen dependencias geográficas asociadas al departamento
            /// (Municipios que pertenecen a este departamento)
            return await _context.Municipalities
                .AsNoTracking()
                .AnyAsync(m => m.DepartmentId == departmentId);
        }
        private async Task<bool> CustomCodeExists(string customCode, int? excludeId = null)
        {
            if (string.IsNullOrWhiteSpace(customCode))
                return false;

            var query = _context.Departments.Where(d => d.CustomCode == customCode);

            if (excludeId.HasValue)
                query = query.Where(d => d.DepartmentId != excludeId.Value);

            return await query.AnyAsync();
        }
        private List<SelectListItem> GetStatusOptions()
        {
            return Enum.GetValues(typeof(VisibilityStatus))
                .Cast<VisibilityStatus>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(),
                    Value = ((int)v).ToString()
                }).ToList();
        }
        private async Task<List<SelectListItem>> GetCountriesSelectListAsync(int? selectedId = null)
        {
            var countries = await _context.Countries
                .Where(c => c.VisibilityStatus == VisibilityStatus.Activo)
                .OrderBy(c => c.CountryName)
                .AsNoTracking()
                .ToListAsync();

            return countries.Select(c => new SelectListItem
            {
                Value = c.CountryId.ToString(),
                Text = c.CountryName,
                Selected = selectedId.HasValue && c.CountryId == selectedId.Value
            }).ToList();
        }

        [AcceptVerbs("GET", "POST")]
        [Route("VerifyCustomCode")]
        public async Task<IActionResult> VerifyCustomCode(string customCode, int? departmentId)
        {
            if (string.IsNullOrWhiteSpace(customCode))
                return Json(true); // Válido si está vacío

            var exists = await CustomCodeExists(customCode, departmentId);
            return Json(!exists);
        }

        [HttpGet("Error")]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}
