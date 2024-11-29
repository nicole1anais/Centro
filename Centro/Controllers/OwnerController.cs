using Centro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Centro.Controllers
{
    [Authorize(Roles = "Owner")]
    public class OwnerController : Controller
    {
        private readonly AppDbContext _context;

        public OwnerController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            DateTime fechaHoy = DateTime.Now;
            DateTime fechaMesAnterior = fechaHoy.AddMonths(-1);

            ViewBag.numDeCitasMes = _context.Citas.Where(x => x.Fecha >= fechaMesAnterior && x.Fecha <= fechaHoy && x.Disponible == false).ToList().Count();
            ViewBag.recaudadcionMes = _context.Citas.Where(x => x.Fecha >= fechaMesAnterior && x.Fecha <= fechaHoy && x.Disponible == false).Sum(x => x.Precio);

            var citasPorProfesionales = _context.Citas.Include(x => x.Profesionales).Where(x => x.Disponible == false).
                 GroupBy(cita => cita.Profesionales.Nombre) // Agrupar por el nombre del médico
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
    }
}