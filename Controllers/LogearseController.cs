using System.Security.Claims;
using System.Threading.Tasks;
using eticket.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace eticket.Controllers
{
    public class LogearseController(TicketsDBContext context) : Controller
    {
        private readonly TicketsDBContext context = context;

        [HttpGet("/logearse")]
        public ActionResult Logearse()
        {
            return View();
        }

        [HttpPost("/logearse")]
        public async Task<IActionResult> Logearse(string usuario, string contraseña)
        {
            var user = context.Usuarios.FirstOrDefault(u => u.Usuario1 == usuario && u.Activo == true);

            if (user != null && BCrypt.Net.BCrypt.Verify(contraseña, user.Contraseña))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new(ClaimTypes.Name, user.Usuario1),
                    new(ClaimTypes.Role, user.Rol ?? "Usuario"),
                };

                var claimIdentity = new ClaimsIdentity(claims, "NerusTicketCookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync("NerusTicketCookieAuth", new ClaimsPrincipal(claimIdentity), authProperties);
                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Usuario o contraseña incorrectos.";
            return View();
        }
        
        [HttpGet("/cerrar-sesion")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("NerusTicketCookieAuth");
            return RedirectToAction("Logearse");
        }

    }
}
