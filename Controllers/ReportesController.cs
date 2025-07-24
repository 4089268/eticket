using System.Data.Entity;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using eticket.Adapters;
using eticket.Data;
using eticket.Models;
using eticket.ViewModels;
using FluentValidation;

namespace eticket.Controllers
{
    [Route("/{Controller}")]
    public class ReportesController(ILogger<ReportesController> logger, TicketsDBContext context, IValidator<ReporteRequest> validator) : Controller
    {
        private readonly ILogger<ReportesController> logger = logger;
        private readonly TicketsDBContext ticketsDBContext = context;
        private readonly IValidator<ReporteRequest> reportValidator = validator;
        private readonly int pageSize = 10;


        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            var totalItems = ticketsDBContext.OprReportes.Count();

            var reportes = ticketsDBContext.OprReportes
                .OrderByDescending(r => r.FechaRegistro)
                .Include(r => r.IdEstatusNavigation)
                .Include(r => r.IdGeneroNavigation)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;

            return View("Reportes", reportes);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlmacenarReporte([FromForm] ReporteRequest model)
        {
            // Validate the request
            var validation = this.reportValidator.Validate(model);
            if (!validation.IsValid)
            {
                return UnprocessableEntity(new
                {
                    errors = validation.Errors.Select(e => new {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
            }

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
            logger.LogInformation("Nuevo reporte creado con ID {ReporteId} por usuario {Usuario}", reporte.IdReporte, User.Identity?.Name ?? "Desconocido");
            return Ok(new {
                success = true,
                message = "Reporte guardado correctamente",
                folio = reporte.Folio
            });
        }

        [HttpGet("{folioReporteArg}")]
        public IActionResult MostrarReporte([FromRoute] string folioReporteArg)
        {
            var folioReporte = long.TryParse(folioReporteArg, out long f) ? f : -1;
            if (folioReporte == -1)
            {
                ViewBag.ErrorTitle = "Folio invalido";
                ViewBag.ErrorMessage = "El formato del folio no es valido";
                return View("NotFound");
            }

            this.logger.LogInformation("Mostrando reporte con folio: {folio}", folioReporte);

            var reporte = this.ticketsDBContext.OprReportes.FirstOrDefault(item => item.Folio == folioReporte);
            if (reporte == null)
            {
                ViewBag.ErrorMessage = $"No existe el reporte con folio : {folioReporte}";
                return View("NotFound");
            }

            return View(reporte);
        }

        #region PartialViews
        [HttpGet("partial-view/menu")]
        public IActionResult NuevoReporteMenuPartialView()
        {
            try
            {
                var elements = this.ticketsDBContext.CatReportes.Where(item => item.IdReporte > 0).ToList();
                return PartialView("~/Views/Reportes/Partials/NewFormMenu.cshtml", elements);
            }
            catch (Exception err)
            {
                this.logger.LogError(err, "Error al obtener los elementos del menú de reporte");
                ViewData["ErrorTitle"] = "Error al obtener los elementos del menú de reporte";
                ViewData["ErrorMessage"] = err.Message;
                return PartialView("~/Views/Shared/_ErrorAlert.cshtml");
            }
        }

        [HttpGet("partial-view/formulario")]
        public IActionResult NuevoReporteFormPartialView(int tipoReporteId)
        {
            try
            {
                // * cargar tipo reporte seleccionado
                var _tipoReporte = this.ticketsDBContext.CatReportes
                    .FirstOrDefault(item => item.IdReporte == tipoReporteId) ?? throw new ArgumentException($"No se encontró el tipo de reporte con Id {tipoReporteId}");
                
                // * cargar catalog the estatus
                var estatusList = this.ticketsDBContext.CatEstatuses
                    .OrderBy(e => e.Descripcion)
                    .Select(e => new SelectListItem
                    {
                        Value = e.IdEstatus.ToString(),
                        Text = e.Descripcion
                    }).ToList();
                ViewBag.EstatusList = estatusList;

                // * cargar catalog tipo entradas
                var tipoEntradaList = this.ticketsDBContext.CatTipoEntrada
                    .OrderBy(item => item.Descripcion)
                    .Select(e => new SelectListItem
                    {
                        Value = e.IdTipoentrada.ToString(),
                        Text = e.Descripcion
                    }).ToList();
                ViewBag.TipoEntradaList = tipoEntradaList;

                var reporteRequest = new ReporteRequest()
                {
                    TipoReporte = _tipoReporte,
                    IdTipoEntrada = 1,
                    IdEstatus = 1
                };

                return PartialView("~/Views/Reportes/Partials/NuevoReporteForm.cshtml", reporteRequest);
            }
            catch (Exception err)
            {
                this.logger.LogError(err, "Error al generar el formulario del registro");
                ViewData["ErrorTitle"] = "Error al generar el formulario del registro";
                ViewData["ErrorMessage"] = err.Message;
                return PartialView("~/Views/Shared/_ErrorAlert.cshtml");
            }
        }
        #endregion
    }
}
