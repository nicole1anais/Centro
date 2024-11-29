using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Centro.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class AdministradorController : Controller
    {
        private readonly AppDbContext _context;

        public AdministradorController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaMesAnterior = fechaHoy.AddMonths(-1);

            ViewBag.numDeCitasMes = _context.Citas.Where(x => x.Disponible == false).ToList().Count(); // && x.Fecha>= fechaMesAnterior && x.Fecha<=fechaHoy
            ViewBag.recaudadcionMes = _context.Citas.Where(x => x.Disponible == false).Sum(x => x.Precio); //&& x.Fecha >= fechaMesAnterior && x.Fecha <= fechaHoy
            ViewBag.numContactos = _context.Contactos.ToList().Count();

            var citasPorProfesionales = _context.Citas.Include(x => x.Profesionales).Where(x => x.Disponible == false).
                 GroupBy(cita => cita.Profesionales.Nombre) // Agrupar por el nombre 
                 .Select(group => new
                 {
                     Profesionales = group.Key,
                     NumeroCitas = group.Count()
                 })
         .ToList();
            ViewBag.nombresProfesionales = citasPorProfesionales.Select(r => r.Profesionales).ToList();
            ViewBag.citasPorProfesionales = citasPorProfesionales.Select(r => r.NumeroCitas).ToList();

            ViewBag.numProfesionales = _context.Profesionales.ToList().Count();

            return View();
        }
        public IActionResult ViewProfesionales()
        {
            var listProfesionales = _context.Profesionales.ToList();
            return View(listProfesionales);
        }
        public async Task<IActionResult> DetailsProfesionales(int? id)
        {
            if (id == null || _context.Profesionales == null)
            {
                return NotFound();
            }

            var Profesionales = await _context.Profesionales
                .FirstOrDefaultAsync(m => m.Id == id);
            if (Profesionales == null)
            {
                return NotFound();
            }

            return View(Profesionales);
        }
    }
}