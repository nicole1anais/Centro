using Microsoft.AspNetCore.Mvc;

namespace CentroMedicoIntegracion.Controllers
{
    public class Historia : Controller
    {
        public IActionResult Infohistoria()
        {
            return View();
        }
    }
}