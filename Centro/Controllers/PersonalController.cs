using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Centro.Controllers
{
    public class PersonalController : Controller
    {

        private readonly AppDbContext _context;

        public PersonalController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Infoprofesionales()
        {
            var Profesionales = _context.Profesionales.Include(x => x.Especialidad).ToList();

            return View(Profesionales);
        }
    }
}