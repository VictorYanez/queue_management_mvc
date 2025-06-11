using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using queue_management.Data;
using queue_management.Enums;
using queue_management.Models;
using queue_management.Services;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace queue_management.Controllers
{
    [Route("Countries")]
    public class CountriesController : Controller
    {
        private readonly ApplicationDBContext _context;
        private readonly ILogger<CountriesController> _logger;
        private readonly IGeographyService _geoService;

        public CountriesController(ApplicationDBContext context, ILogger<CountriesController> logger,
            IGeographyService geoService)
        {
            _context = context;
            _logger = logger;
            _geoService = geoService;
        }

        #region Index & Details Actions
        // ----- 🔥 ----- CRUD Actions ----- 🔥 ----- //

        // GET: Countries
        [HttpGet("")]
        public async Task<IActionResult> Index()
        {
            try
            {
                var countries = await _context.Countries
                    .Where(c => c.VisibilityStatus == VisibilityStatus.Activo)
                    .OrderBy(c => c.CountryName)
                    .AsNoTracking()
                    .ToListAsync();

                return View(countries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar países");
                return RedirectToAction(nameof(Error));
            }
        }

        // GET: Countries/Details
        [HttpGet("Details/{id:int}")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            try
            {
                var country = await _context.Countries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CountryId == id);

                if (country == null)
                {
                    return NotFound();
                }
                ViewData["VisibilityStatusOptions"] = GetVisibilityStatusOptions();
                return View(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar país ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }
        #endregion

        #region Create Actions
        // GET: Countries/Create
        [HttpGet("Create")]
        //[Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            ViewData["VisibilityStatusOptions"] = GetVisibilityStatusOptions();
            return View(new Country
            {
                VisibilityStatus = VisibilityStatus.Activo,
                IsDefault = false
            });
        }

        // POST: Countries/Create
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("CountryId,CountryName,CustomCode,VisibilityStatus,IsDefault")] Country country)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Si es el primer país, marcarlo como default
                    if (!await _context.Countries.AnyAsync())
                    {
                        country.IsDefault = true;
                    }
                    // Si se marca como default, actualizar los demás
                    else if (country.IsDefault)
                    {
                        await _geoService.SetDefaultCountry(country.CountryId);
                    }

                    _context.Add(country);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                // Si el modelo no es válido, volver a mostrar el formulario
                ViewData["VisibilityStatusOptions"] = GetVisibilityStatusOptions();
                return View(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear país");
                ModelState.AddModelError("", "Ocurrió un error al crear el país");
                return RedirectToAction(nameof(Error));
            }
        }
        #endregion

        // GET: Countries/Edit
        //[Authorize(Roles = "Admin")]
        [HttpGet("Edit/{id:int}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            try
            {
                var country = await _context.Countries.FindAsync(id);
                if (country == null)
                {
                    return NotFound();
                }

                ViewData["VisibilityStatusOptions"] = GetVisibilityStatusOptions();
                return View(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar país ID {id} para edición");
                return RedirectToAction(nameof(Error));
            }
        }

        #region Edit Actions
        // POST: Countries/Edit
        [HttpPost("Edit/{id:int}")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("CountryId,CountryName,CustomCode,VisibilityStatus,IsDefault,RowVersion")] Country country)
        {
            if (id != country.CountryId)
            {
                return NotFound();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    if (country.IsDefault)
                    {
                        await _geoService.SetDefaultCountry(country.CountryId);
                    }

                    _context.Update(country);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "País actualizado exitosamente";
                    return RedirectToAction(nameof(Index));
                }
                // Si el modelo no es válido, volver a mostrar el formulario
                ViewData["VisibilityStatusOptions"] = GetVisibilityStatusOptions();
                return View(country);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!CountryExists(id))
                {
                    return NotFound();
                }
                _logger.LogError(ex, $"Error de concurrencia al editar país ID {id}");
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al editar país ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }
        #endregion

        #region Delete Actions
        // GET: Countries/Delete
        [ActionName("Delete")]
        [HttpGet("Delete/{id:int}")]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || id <= 0)
            {
                return NotFound();
            }

            try
            {
                var country = await _context.Countries
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.CountryId == id);
                if (country == null)
                {
                    return NotFound();
                }
                ViewData["VisibilityStatusOptions"] = GetVisibilityStatusOptions();
                return View(country);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al cargar país ID {id} para eliminación");
                return RedirectToAction(nameof(Error));
            }
        }

        // POST: Countries/Delete/5
        [HttpPost("Delete/{id:int}")]
        [ActionName("DeleteConfirmed")]
        [ValidateAntiForgeryToken]
        //[Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var country = await _context.Countries.FindAsync(id);
                if (country == null)
                {
                    return NotFound();
                }

                if (await _context.Departments.AnyAsync(d => d.CountryId == id))
                {
                    TempData["ErrorMessage"] = "No se puede eliminar el país porque tiene departamentos asociados.";
                    return RedirectToAction(nameof(Delete), new { id });
                }

                _context.Countries.Remove(country);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error al eliminar país ID {id}");
                return RedirectToAction(nameof(Error));
            }
        }
        #endregion

        #region Helper Methods
        // ----- 🔥 ----- Helper Methods ----- 🔥 ----- //
        private bool CountryExists(int id)
        {
            return _context.Countries.Any(e => e.CountryId == id);
        }

        private List<SelectListItem> GetVisibilityStatusOptions()
        {
            return Enum.GetValues(typeof(VisibilityStatus))
                .Cast<VisibilityStatus>()
                .Select(v => new SelectListItem
                {
                    Text = v.ToString(), // Puedes aplicar un método de localización si usas uno
                    Value = ((int)v).ToString()
                }).ToList();
        }

        // GET  /Countries/VerifyCustomCode
        // POST /Countries/VerifyCustomCode
        [AcceptVerbs("GET", "POST")]
        [Route("VerifyCustomCode")]
        public async Task<IActionResult> VerifyCustomCode(string customCode, int countryId)
        {
            if (string.IsNullOrWhiteSpace(customCode))
            {
                return Json(true); // Si está vacío, es válido
            }

            var exists = await _context.Countries
                .AnyAsync(c => c.CustomCode == customCode && c.CountryId != countryId);

            return Json(!exists); // true si NO existe → válido
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]

        [HttpGet("Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       
        #endregion
    }
}
