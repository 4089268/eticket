using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using eticket.Adapters;
using eticket.Data;
using eticket.Models;
using eticket.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace eticket.Controllers
{
    public class ReportesController(ILogger<ReportesController> logger, TicketsDBContext context) : Controller
    {
        private readonly ILogger<ReportesController> logger = logger;
        private readonly TicketsDBContext ticketsDBContext = context;

        private readonly int pageSize = 25;


        [HttpGet]
        public IActionResult Index(int page = 1 )
        {
            var totalItems = ticketsDBContext.OprReportes.Count();

            var reportes = ticketsDBContext.OprReportes
                .OrderByDescending(r => r.FechaRegistro)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;

            return View("Reportes", reportes);
        }

        [HttpGet("nuevo")]
        public ActionResult Nuevo()
        {
            return View(new ReporteRequest());
        }

        [HttpPost("nuevo")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Nuevo(ReporteRequest model)
        {
            if (ModelState.IsValid)
            {
                var reporte = ReporteAdapter.ToEntity(model);
                reporte.FechaRegistro = DateTime.UtcNow;
                var _userId = HttpContext.User.Claims?.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
                if (_userId == null)
                {
                    throw new Exception("No se pudo obtener el identificador del usuario.");
                }
                reporte.IdGenero = Convert.ToInt32(_userId);

                this.ticketsDBContext.OprReportes.Add(reporte);
                await ticketsDBContext.SaveChangesAsync();
                TempData["Success"] = "Reporte guardado correctamente";
                TempData["SwalMessage"] = $"Reporte generado exitosamente con folio: {reporte.Folio}";
                logger.LogInformation("Nuevo reporte creado con ID {ReporteId} por usuario {Usuario}", reporte.IdReporte, User.Identity?.Name ?? "Desconocido");
                return RedirectToAction("Nuevo");
            }
            return View(model);
        }

    }
}
