using Centro.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Centro.Controllers
{
    public class AuthController : Controller
    {
        private readonly AppDbContext _context;

        public AuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: AuthController
        public IActionResult Login()
        {
            // Verificar si ya está autenticado
            if (User.Identity.IsAuthenticated)
            {
                var user = _context.Usuarios.FirstOrDefault(t => t.Rut.Equals(User.Identity.Name));
                if (user != null)
                {
                    if (user.Rol == "Owner")
                    {
                        return RedirectToAction("Index", "Owner");
                    }
                    else if (user.Rol == "Administrador")
                    {
                        return RedirectToAction("Index", "Administrador");
                    }
                }
                // Si el rol no es conocido, redirige a una página por defecto
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> Login(string EmailForm, string pass)
        {
            var us = _context.Usuarios.FirstOrDefault(x => x.Email == EmailForm);

            if (us != null)
            {
                // Usuario encontrado
                if (VerificarPass(pass, us.PasswordHash, us.PasswordSalt))
                {
                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, us.Rut),
                new Claim(ClaimTypes.Role, us.Rol)
            };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal,
                        new AuthenticationProperties { IsPersistent = true });

                    if (us.Rol == "Owner")
                    {
                        return RedirectToAction("Index", "Owner");
                    }
                    else if (us.Rol == "Administrador")
                    {
                        return RedirectToAction("Index", "Administrador");
                    }
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Contraseña Incorrecta");
                }
            }
            else
            {
                ModelState.AddModelError("", "Usuario no Encontrado!");
            }

            return View();
        }

        [HttpPost]
        public IActionResult Create(string Rut, string Nombre, string Email, string Password, string Rol, bool isBlock)
        {
            var U = new Usuario
            {
                Rut = Rut,
                Nombre = Nombre,
                Email = Email,
                Rol = Rol,
                isBlock = isBlock
            };

            CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);

            U.PasswordHash = passwordHash;
            U.PasswordSalt = passwordSalt;
            _context.Usuarios.Add(U);
            _context.SaveChanges();
            return RedirectToAction("Index", "Usuarios");
        }

        [HttpPost]
        public IActionResult EditUser(string Rut, string Nombre, string Email, string Password, string Rol, bool isBlock)
        {
            var U = _context.Usuarios.FirstOrDefault(x => x.Rut == Rut);

            if (U != null)
            {
                U.Nombre = Nombre;
                U.Email = Email;
                U.Rut = Rut;
                U.Rol = Rol;
                U.isBlock = isBlock;

                CreatePasswordHash(Password, out byte[] passwordHash, out byte[] passwordSalt);

                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;
                _context.Usuarios.Update(U);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "Usuarios");
        }

        //CREDENCIAALESSSSS
        public IActionResult CreateAdmin()
        {
            try
            {
                var U = new Usuario
                {
                    Nombre = "admin",
                    Email = "gonuprofesionales@gmail.com",
                    Rut = "admin",
                    Rol = "Administrador",
                    isBlock = false
                };

                CreatePasswordHash("admin", out byte[] passwordHash, out byte[] passwordSalt);

                U.PasswordHash = passwordHash;
                U.PasswordSalt = passwordSalt;
                _context.Usuarios.Add(U);
                _context.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                // Manejo de excepción
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public IActionResult RegisterPost(string Email, string Rut, string Nombre, string Password, string Rol)
        {
            var us = _context.Usuarios.FirstOrDefault(u => u.Rut.Equals(Rut));
            if (us != null)
            {
                // El usuario ya está registrado
                ModelState.AddModelError("", "RUT Ya Registrado!");
                return RedirectToAction("Login", "Auth");
            }

            return RedirectToAction("Index", "Home");
        }

        public async Task<IActionResult> LogOut()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        public void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public bool VerificarPass(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var pass = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return pass.SequenceEqual(passwordHash);
            }
        }
    }
}