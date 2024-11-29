using Centro.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Mail;
using System.Net;

namespace Centro.Controllers
{
    public class CitasController : Controller
    {
        private readonly AppDbContext _context;

        public CitasController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Solicitud()
        {
            ViewBag.Categorias = _context.Especialidad.ToList();
            return View();
        }

        [HttpGet]
        public ActionResult ObtenerHorasPorFecha(DateTime fecha, int ProfesionalesId)
        {
            var horas = _context.Citas
                .Where(x => x.Fecha.Date == fecha.Date && x.Disponible == true && x.ProfesionalesId == ProfesionalesId)
                .Select(m => m.Hora)
                .ToList();

            string jsonHoras = JsonConvert.SerializeObject(horas);
            return Content(jsonHoras, "application/json");
        }

        [HttpGet]
        public ActionResult ObtenerFechasPorProfesionales(int idProfesionales)
        {
            var citasAgrupadas = _context.Citas
             .Where(x => x.ProfesionalesId == idProfesionales && x.Disponible == true && x.Fecha > DateTime.Now)
             .GroupBy(m => m.Fecha)
             .Select(group => new
             {
                 Fecha = group.Key,
                 Horas = group.Select(item => item.Hora).ToList()
             })
             .ToList();


            // Serializar la lista de médicos a formato JSON
            string jsonProfesionales = JsonConvert.SerializeObject(citasAgrupadas);

            // Devolver la respuesta JSON
            return Content(jsonProfesionales, "application/json");
        }

        [HttpGet]
        public IActionResult cargarHoras(int idProfesionales)
        {
            var Profesionales = _context.Profesionales.FirstOrDefault(x => x.Id == idProfesionales);
            if (Profesionales == null) return NotFound();

            List<DateTime> diasHabiles = ObtenerDiasHabiles(); // Debes definir este método
            string[] Horas = { "08:00", "08:30", "09:00", "09:30", "10:00", "10:30", "11:00", "11:30", "12:00", "12:30", "13:00", "14:30", "15:00", "15:30", "16:00", "17:30" };

            DateTime primerDiaDelMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime ultimoDiaDelMes = primerDiaDelMes.AddMonths(1).AddDays(-1);

            var estanAgregadas = _context.Citas
                .Where(x => x.ProfesionalesId == idProfesionales && x.Fecha >= primerDiaDelMes && x.Fecha <= ultimoDiaDelMes)
                .ToList();

            if (!estanAgregadas.Any())
            {
                foreach (var dia in diasHabiles)
                {
                    foreach (var hora in Horas)
                    {
                        var nuevaCita = new Citas
                        {
                            Disponible = true,
                            Hora = hora,
                            Fecha = dia,
                            ProfesionalesId = idProfesionales
                        };
                        _context.Citas.Add(nuevaCita);
                    }
                }
                _context.SaveChanges();
            }

            return Ok();
        }

        public List<DateTime> ObtenerDiasHabiles()
        {
            List<DateTime> diasHabiles = new List<DateTime>();
            DateTime fechaActual = DateTime.Today;
            int diasEnElMes = DateTime.DaysInMonth(fechaActual.Year, fechaActual.Month);

            for (int i = 1; i <= diasEnElMes; i++)
            {
                DateTime fecha = new DateTime(fechaActual.Year, fechaActual.Month, i);

                // Verificamos si el día es hábil (lunes a viernes)
                if (EsDiaHabil(fecha))
                {
                    diasHabiles.Add(fecha);
                }
            }

            return diasHabiles;
        }
        public bool EsDiaHabil(DateTime fecha)
        {
            // Verificamos si el día es sábado o domingo
            if (fecha.DayOfWeek == DayOfWeek.Saturday || fecha.DayOfWeek == DayOfWeek.Sunday)
            {
                return false;
            }

            // Puedes agregar lógica adicional para manejar días festivos si es necesario

            return true;
        }

        // GET: Citas
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> List()
        {
            return _context.Citas != null ?
                        View(_context.Citas.Include(x => x.Profesionales).Include(x => x.Especialidad).Where(x => x.Disponible == false).ToList()) :
                        Problem("Entity set 'AppDbContext.Citas'  is null.");
        }

        // GET: Citas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Citas == null)
            {
                return NotFound();
            }

            var citas = await _context.Citas.Include(x => x.Profesionales).Include(x => x.Especialidad)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (citas == null)
            {
                return NotFound();
            }
            citas.Fecha = citas.Fecha.Date;
            return View(citas);
        }

        // GET: Citas/Create
        [Authorize(Roles = "Administrador")]
        public IActionResult Create()
        {
            ViewBag.Categorias = _context.Especialidad.ToList();
            ViewBag.Profesionales = _context.Profesionales.ToList();
            return View();
        }

        public IActionResult Aprobada(int id)
        {
            if (TempData.ContainsKey("AutorizacionTemporal") && (bool)TempData["AutorizacionTemporal"])
            {
                ViewData["AutorizacionTemporal"] = true;
                TempData.Remove("AutorizacionTemporal"); // Limpiar TempData
            }
            ViewBag.cod = id;
            return View();

        }

        // POST: Citas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Rut,Nombre,Apellido,Correo,Telefono,Fecha,Hora,EspecialidadId,ProfesionalesId,Precio")] Citas citas)
        {
            var Cita = _context.Citas.Include(x => x.Profesionales).Where(x => x.Disponible == true
                                           && x.ProfesionalesId == citas.ProfesionalesId
                                           && x.Fecha == citas.Fecha
                                           && x.Hora == citas.Hora
                                    ).FirstOrDefault();
            Cita.Rut = citas.Rut;
            Cita.Nombre = citas.Nombre;
            Cita.Apellido = citas.Apellido;
            Cita.Correo = citas.Correo;
            Cita.Telefono = citas.Telefono;
            Cita.EspecialidadId = citas.EspecialidadId;
            Cita.Precio = citas.Precio;

            Cita.Disponible = false;
            var fechaFormateada = Cita.Fecha.ToString("dd/MM/yyyy");

            var body = "Nos complace informarle que su cita ha sido programada con éxito con el/la Profesionales: " + Cita.Profesionales.Nombre + " El dia: " + fechaFormateada + " A las: " + Cita.Hora;

            _context.Citas.Update(Cita);
            await _context.SaveChangesAsync();
            enviarCorreo("CITA AGENDADA CON EXITO", body, Cita.Correo);
            TempData["AutorizacionTemporal"] = true;

            return RedirectToAction("Aprobada", new { Id = Cita.Id });

        }

        //CAMBIAR CORREOO

        public void enviarCorreo(string asunto, string body, string correoDestinario)
        {

            MailMessage message = new MailMessage("gonuprofesionales@gmail.com", correoDestinario);
            message.Subject = asunto;
            message.Body = body;

            SmtpClient client = new SmtpClient("smtp.gmail.com");

            client.Port = 587;
            client.Credentials = new NetworkCredential("gonuprofesionales@gmail.com", "ahnv ewzs vugz hdpn");
            client.EnableSsl = true;

            client.Send(message);

        }

        public IActionResult Boleta(int id)
        {

            if (id == null || _context.Citas == null)
            {
                return NotFound();
            }

            var citas = _context.Citas.Include(x => x.Profesionales).Include(x => x.Especialidad)
                .FirstOrDefault(m => m.Id == id);
            if (citas == null)
            {
                return NotFound();
            }

            return View(citas);

        }

        // GET: Citas/Edit/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Citas == null)
            {
                return NotFound();
            }

            var citas = await _context.Citas.FindAsync(id);
            if (citas == null)
            {
                return NotFound();
            }
            return View(citas);
        }

        // POST: Citas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Rut,Nombre,Apellido,Correo,Telefono,Fecha,Hora,EspecialidadId,ProfesionalesId,Precio,Bono")] Citas citas)
        {
            if (id != citas.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(citas);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CitasExists(citas.Id))
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
            return View(citas);
        }

        // GET: Citas/Delete/5
        [Authorize(Roles = "Administrador")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Citas == null)
            {
                return NotFound();
            }

            var citas = await _context.Citas
                .FirstOrDefaultAsync(m => m.Id == id);
            if (citas == null)
            {
                return NotFound();
            }

            return View(citas);
        }

        // POST: Citas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Citas == null)
            {
                return Problem("Entity set 'AppDbContext.Citas'  is null.");
            }
            var citas = _context.Citas.Include(x => x.Profesionales).Where(x => x.Id == id).FirstOrDefault();
            if (citas != null)
            {
                var fechaFormateada = citas.Fecha.ToString("dd/MM/yyyy");
                var body = "Lamentamos informarle que su cita programada con el/la Profesionales: " + citas.Profesionales.Nombre + " para el dia: " + fechaFormateada + " A las: " + citas.Hora + " ha sido cancelada. Entendemos que pueden surgir imprevistos y agradecemos su comprensión.\r\n\r\nSi desea reprogramar su cita o tiene alguna pregunta, no dude en ponerse en contacto con nosotros. Estamos aquí para ayudarle en lo que necesite.";
                enviarCorreo("Cita eliminada CENTRO Profesionales", body, citas.Correo);
                citas.Disponible = true;
                _context.Citas.Update(citas);
            }

            await _context.SaveChangesAsync();
            TempData["Mensaje"] = "La cita fue eliminada con exito.";

            return RedirectToAction("List");
        }

        private bool CitasExists(int id)
        {
            return (_context.Citas?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}