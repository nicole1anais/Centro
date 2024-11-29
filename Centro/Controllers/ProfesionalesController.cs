using Centro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Centro.Controllers
{
    public class ProfesionalesController : Controller
    {
        private readonly AppDbContext _context;

        public ProfesionalesController(AppDbContext context)
        {
            _context = context;
        }
      
        // GET
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Index()
        {
            return _context.Profesionales != null ?
                 View(await _context.Profesionales.ToListAsync()) :
                 Problem("Entity set 'AppDbContext.Usuarios'  is null.");
        }
        [HttpGet]
        public ActionResult ObtenerProfesionalesPorEspecialidad(int especialidadId)
        {
            var Profesionales = _context.Profesionales
                .Where(x => x.EspecialidadId == especialidadId)
                .Select(m => new { m.Id, m.Nombre })
                .ToList();

            // Serializar la lista de médicos a formato JSON
            string jsonProfesionales = JsonConvert.SerializeObject(Profesionales);

            // Devolver la respuesta JSON
            return Content(jsonProfesionales, "application/json");
        }


        // GET: Medicos/Details/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Profesionales == null)
            {
                return NotFound();
            }

            var Profesionales = await _context.Profesionales
                .Include(m => m.Especialidad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Profesionales == null)
            {
                return NotFound();
            }

            return View(Profesionales);
        }

        // GET: Medicos/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Especialidad.ToList();

            return View();
        }

        // POST: Medicos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Rut,Imagen,Nombre,Email,EspecialidadId,SedeEgreso,Diplomados,Latitud,Longitud")] Profesionales Profesionales)
        {

            _context.Add(Profesionales);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }

        // GET: Profesionales/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Profesionales == null)
            {
                return NotFound();
            }

            var Profesionales = await _context.Profesionales.FindAsync(id);
            if (Profesionales == null)
            {
                return NotFound();
            }
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "IdEspecialidad", "IdEspecialidad", Profesionales.EspecialidadId);
            ViewBag.Categorias = _context.Especialidad.ToList();

            return View(Profesionales);
        }

        // POST: Medicos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rut,Imagen,Nombre,Email,EspecialidadId,SedeEgreso,Diplomados")] Profesionales Profesionales)
        {
            if (id != Profesionales.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(Profesionales);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProfesionalesExists(Profesionales.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["EspecialidadId"] = new SelectList(_context.Especialidad, "IdEspecialidad", "IdEspecialidad", Profesionales.EspecialidadId);
            return View(Profesionales);
        }

        // GET: Profesionales/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Profesionales == null)
            {
                return NotFound();
            }

            var Profesionales = await _context.Profesionales
                .Include(m => m.Especialidad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Profesionales == null)
            {
                return NotFound();
            }

            return View(Profesionales);
        }

        // POST: Profesionales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Profesionales == null)
            {
                return Problem("Entity set 'AppDbContext.Profesionales'  is null.");
            }
            var Profesionales = await _context.Profesionales.FindAsync(id);
            if (Profesionales != null)
            {
                var citasDeProfesionales = _context.Citas.Where(x => x.ProfesionalesId == Profesionales.Id).ToList();
                if (citasDeProfesionales.Count > 0)
                {
                    for (int i = 0; i < citasDeProfesionales.Count; i++)
                    {
                        _context.Citas.Remove(citasDeProfesionales[i]);
                        _context.SaveChanges();
                    }
                }
                _context.Profesionales.Remove(Profesionales);
            }
            TempData["Mensaje"] = "El medico fue eliminado con exito.";

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProfesionalesExists(int id)
        {
            return (_context.Profesionales?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //MAPA
        [HttpGet]
        public IActionResult GetProfesionales()
        {
            var profesionales = _context.Profesionales
                .Select(p => new {
                    id = p.Id,
                    imagen = p.Imagen,
                    nombre = p.Nombre,
                    especialidad = p.Especialidad.Nombre,
                    email =p.Email,
                    latitud = p.Latitud,
                    longitud = p.Longitud
                }).ToList();

            return Json(profesionales);
        }

    }
}