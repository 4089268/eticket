using System.Security.Claims;
using System.Threading.Tasks;
using eticket.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;

namespace eticket.Controllers
{
    public class LogearseController(ILogger<LogearseController> logger, TicketsDBContext context) : Controller
    {
        private readonly ILogger<LogearseController> logger = logger;
        private readonly TicketsDBContext context = context;

        [HttpGet("/logearse")]
        public ActionResult Logearse()
        {
            return View();
        }

        [HttpPost("/logearse")]
        public async Task<IActionResult> Logearse(string usuario, string contraseña)
        {
            var user = context.SysUsuarios.FirstOrDefault(u => u.Usuario == usuario && u.Activo == true);
            
            if (user != null && BCrypt.Net.BCrypt.Verify(contraseña, user.Contraseña))
            {
                // update las connection
                user.UltimoInicioSesion = DateTime.Now;
                context.SysUsuarios.Update(user);
                await context.SaveChangesAsync();

                // make the userIdentiy
                var claims = new List<Claim>
                {
                    new(ClaimTypes.NameIdentifier, user.IdUsuario.ToString()),
                    new(ClaimTypes.Name, user.Usuario),
                    new(ClaimTypes.Role, user.Rol ?? "Usuario"),
                };

                var claimIdentity = new ClaimsIdentity(claims, "NerusTicketCookieAuth");
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true
                };

                await HttpContext.SignInAsync("NerusTicketCookieAuth", new ClaimsPrincipal(claimIdentity), authProperties);
                logger.LogInformation("Usuario {Usuario} inició sesión correctamente.", user.Usuario);
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
