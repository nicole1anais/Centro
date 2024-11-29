using Centro.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Centro.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppDbContext _context;

        public HomeController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {

            var Especialidad = _context.Especialidad.ToList().Count;
            if (Especialidad == 0)
            {
                string[] Especialidades = { "Dentista", "Fisioterapeuta", "Esteticistas", "Cosmetólogos", "Médicos en Medicina Estética Corporal", "Quiroprácticos", "Médicos en Medicina Estética Facial", "Manicuristas" };
                for (int i = 0; i < Especialidades.Length; i++)
                {
                    Especialidad E = new Especialidad();
                    E.Nombre = Especialidades[i];
                    _context.Especialidad.Add(E);
                    _context.SaveChanges();

                }
            }

            var Admin = _context.Usuarios.Where(x => x.Rol == "Administrador").ToList().Count;
            if (Admin == 0)
            {

                return RedirectToAction("CreateAdmin", "Auth");

            }

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}