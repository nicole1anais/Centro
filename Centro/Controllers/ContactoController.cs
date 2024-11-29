using Centro.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Centro.Controllers
{
    public class ContactoController : Controller
    {

        private readonly AppDbContext _context;

        public ContactoController(AppDbContext context)
        {
            _context = context;
        }
        // GET: HomeController1
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult List()
        {
            var contactos = _context.Contactos.ToList();
            return View(contactos);
        }
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Contactos == null)
            {
                return NotFound();
            }

            var Contactos = await _context.Contactos
                .FirstOrDefaultAsync(m => m.idContacto == id);
            if (Contactos == null)
            {
                return NotFound();
            }

            return View(Contactos);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Contactos == null)
            {
                return NotFound();
            }

            var Contactos = await _context.Contactos
                .FirstOrDefaultAsync(m => m.idContacto == id);
            if (Contactos == null)
            {
                return NotFound();
            }

            return View(Contactos);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contactos == null)
            {
                return Problem("Entity set 'AppDbContext.Citas'  is null.");
            }
            var Contactos = await _context.Contactos.FindAsync(id);
            if (Contactos != null)
            {
                _context.Contactos.Remove(Contactos);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public ActionResult Enviar(string Nombre, string Email, string Mensaje)
        {
            var C = new Contacto();
            C.Nombre = Nombre;
            C.Email = Email;
            C.Mensaje = Mensaje;
            _context.Contactos.Add(C);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }
    }
}