using System;
using System.IO;
using System.Data.Entity;
using System.Net.Mime;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using FluentValidation;
using eticket.Adapters;
using eticket.Data;
using eticket.Models;
using eticket.ViewModels;
using eticket.Services;

namespace eticket.Controllers
{
    [Authorize]
    [Route("/{Controller}")]
    public class ReportesController(ILogger<ReportesController> logger, TicketsDBContext context, IValidator<ReporteRequest> validator, ReportService rpservice, IOptions<TempPathSettings> tempPathOptions, TicketsMediaDBContext mediaContext) : Controller
    {
        private readonly ILogger<ReportesController> logger = logger;
        private readonly TicketsDBContext ticketsDBContext = context;
        private readonly TicketsMediaDBContext mediaContext = mediaContext;
        private readonly IValidator<ReporteRequest> reportValidator = validator;
        private readonly ReportService reportService = rpservice;
        private readonly TempPathSettings tempPathSettings = tempPathOptions.Value;
        private readonly int pageSize = 10;


        [HttpGet]
        public IActionResult Index(int page = 1)
        {
            var totalItems = ticketsDBContext.OprReportes.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.CurrentPage = page;

            return View("Reportes");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AlmacenarReporte(ReporteRequest request, List<UploadedFileMetadata> uploadedFiles)
        {
            // Validate the request
            var validation = this.reportValidator.Validate(request);
            if (!validation.IsValid)
            {
                return UnprocessableEntity(new
                {
                    errors = validation.Errors.Select(e => new
                    {
                        field = e.PropertyName,
                        message = e.ErrorMessage
                    })
                });
            }

            // * obtener el usuario de quien lo genera
            request.IdGenero = ObtenerOperadorId();

            long folioReporte = 0;
            // * almacenar el reporte
            try
            {
                // * almacenar reporte
                folioReporte = await this.reportService.AlmacenarReporteInicial(request);
            }
            catch (System.Exception e)
            {
                this.logger.LogError(e, "Error al generar el reporte: {message}", e.Message);
                return Conflict(new
                {
                    Title = "Error al almacenar el reporte",
                    Message = "Error al almacenar el reporte: " + e.Message
                });
            }

            // * almacenar archivos
            if (uploadedFiles?.Any() == true)
            {
                var folderPath = tempPathSettings.Path + "attach-files/";
                foreach (var fileMetadata in uploadedFiles)
                {
                    var filePath = folderPath + fileMetadata.GuidName;

                    try
                    {
                        if (!System.IO.File.Exists(filePath))
                        {
                            logger.LogWarning("Archivo no encontrado: {FilePath}", filePath);
                            continue;
                        }

                        var fileBytes = await System.IO.File.ReadAllBytesAsync(filePath);
                        var record = new OprImagene
                        {
                            FolioReporte = folioReporte,
                            IdInsert = request.IdGenero.ToString(),
                            FechaInsert = DateTime.UtcNow,
                            Documento = fileBytes,
                            FileExtension = Path.GetExtension(filePath),
                            Filesize = fileBytes.Length
                        };

                        mediaContext.OprImagenes.Add(record);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error al procesar el archivo '{FileName}'. Ruta: {FilePath}", fileMetadata.GuidName, filePath);
                    }
                }

                try
                {
                    await mediaContext.SaveChangesAsync();
                    logger.LogInformation("Archivos adjuntos guardados correctamente para el folio {FolioReporte}", folioReporte);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Error al guardar cambios en la base de datos para el folio {FolioReporte}", folioReporte);
                }
            }

            return StatusCode(201, new
            {
                success = true,
                message = "Reporte guardado correctamente",
                folio = folioReporte
            });
        }

        [HttpGet("{folioReporteArg}")]
        public async Task<IActionResult> MostrarReporte([FromRoute] string folioReporteArg)
        {
            var folioReporte = long.TryParse(folioReporteArg, out long f) ? f : -1;
            if (folioReporte == -1)
            {
                ViewBag.ErrorTitle = "Folio invalido";
                ViewBag.ErrorMessage = "El formato del folio no es valido";
                return View("NotFound");
            }

            try
            {
                var reporte = await this.reportService.ObtenerReportePorFolio(folioReporte);

                // * obtener catalog de estatus
                var estatusList = this.ticketsDBContext.CatEstatuses
                    .OrderBy(e => e.Descripcion)
                    .Select(e => new SelectListItem
                    {
                        Value = e.IdEstatus.ToString(),
                        Text = e.Descripcion
                    }).ToList();
                ViewBag.EstatusList = estatusList;

                return View(reporte);
            }
            catch (InvalidOperationException ioe)
            {
                this.logger.LogError(ioe, "Error al obtener el reporte");
                ViewBag.ErrorMessage = $"No existe el reporte con folio : {folioReporte}";
                return View("NotFound");
            }
        }

        [HttpPost("{folioReporteArg}/entrada")]
        public async Task<IActionResult> AlmacenarEntradaReporte([FromRoute] string folioReporteArg, [FromForm] DetReporteRequest request)
        {
            var folioReporte = long.TryParse(folioReporteArg, out long f) ? f : -1;
            if (folioReporte == -1)
            {
                ViewBag.ErrorTitle = "Folio invalido";
                ViewBag.ErrorMessage = "El formato del folio no es valido";
                return View("NotFound");
            }

            try
            {
                // * llenar campos faltantes
                request.IdOperador = ObtenerOperadorId();
                request.Folio = folioReporte;

                // TODO: validar detalle

                // * registrar entrada
                var detReporte = await this.reportService.AlmacenarEntradaReporte(request);

                return StatusCode(201, new
                {
                    Message = "Entrada registrada con exito"
                });
            }
            catch (InvalidOperationException ioe)
            {
                this.logger.LogError(ioe, "Error al obtener el reporte");
                return NotFound(new
                {
                    Message = "El reporte no existe"
                });
            }
            catch (Exception ex)
            {
                this.logger.LogError(ex, "Error al almacenar la entrada: {message}", ex.Message);
                return Conflict(new
                {
                    Title = "Error no controlado al almacenar la entrada",
                    Message = ex.Message
                });
            }
        }

        [HttpPost("upload-attach-file")]
        public async Task<IActionResult> CargarArchivoAdjunto(IFormFile file)
        {
            if (file == null)
            {
                return BadRequest(new { Message = "No se recibió ningún archivo adjunto." });
            }

            // Validar tamaño máximo de 10MB
            const long maxFileSize = 10 * 1024 * 1024; // 10MB en bytes
            if (file.Length > maxFileSize)
            {
                var errors = new Dictionary<string, string>(){ { "file", "El archivo adjunto excede el tamaño máximo permitido de 10MB." }};
                return UnprocessableEntity(new
                {
                    errors = errors.Select( e => new
                    {
                        field = e.Key,
                        message = e.Value
                    })
                });
            }

            // prepare for saving the file
            var attachFileId = Guid.NewGuid();
            var fileExtension = Path.GetExtension(file.FileName);
            var fileName = attachFileId.ToString() + fileExtension;
            var filePath = tempPathSettings.Path + "attach-files/" + fileName;

            if (!Directory.Exists(tempPathSettings.Path + "attach-files"))
            {
                Directory.CreateDirectory(filePath);
            }

            // save the file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            this.logger.LogInformation("Archivo adjunto cargado en ruta: {path}", filePath);

            return Ok(new
            {
                Message = "Archivo adjunto cargado correctamente.",
                FileName = fileName,
                OriginalFileName = file.FileName,
                Size = file.Length
            });
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
                    IdTipoReporte = _tipoReporte.IdReporte,
                    IdTipoEntrada = 1, // Call center
                    IdEstatus = 1 // Abierto
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

        [HttpGet("partial-view/for-new-det-report")]
        public IActionResult FormularioNuevoDetReporte()
        {
            // * obtener catalog estatus
            var estatusList = this.ticketsDBContext.CatEstatuses.ToList().Select(item => new SelectListItem
            {
                Value = item.IdEstatus.ToString(),
                Text = item.Descripcion
            }).ToList();
            ViewBag.EstatusList = estatusList;

            var request = new DetReporteRequest()
            {
                IdEstatus = 2 // En proceso
            };
            return PartialView("~/Views/Reportes/Partials/NuevoDetReporteForm.cshtml", request);
        }

        [HttpGet("partial-view/ultimos-reportes")]
        public IActionResult TablaUltimosReportes()
        {
            try
            {
                var reportes = ticketsDBContext.OprReportes
                    .OrderByDescending(r => r.FechaRegistro)
                    .Include(r => r.IdEstatusNavigation)
                    .Include(r => r.IdGeneroNavigation)
                    .Take(pageSize)
                    .ToList();
                return PartialView("~/Views/Reportes/Partials/UltimosReportesTable.cshtml", reportes);
            }
            catch (Exception err)
            {
                this.logger.LogError(err, "Error al obtener los ultimos reportes");
                ViewData["ErrorTitle"] = "Error al obtener los ultimos reportes";
                ViewData["ErrorMessage"] = err.Message;
                return PartialView("~/Views/Shared/_ErrorAlert.cshtml");
            }
        }
        #endregion


        #region Private functions
        private int ObtenerOperadorId()
        {
            // * ontener el usuairo de quien lo genera
            var _userId = HttpContext.User.Claims?.FirstOrDefault(item => item.Type == ClaimTypes.NameIdentifier)?.Value;
            if (_userId == null)
            {
                throw new Exception("No se pudo obtener el identificador del usuario.");
            }
            return Convert.ToInt32(_userId);
        }
        #endregion
    }
}
