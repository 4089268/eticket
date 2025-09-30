using System;
using System.IO;
using System.Text;
using System.Data.Entity;
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
using eticket.DTO;

namespace eticket.Controllers
{
    [Authorize]
    [Route("/{Controller}")]
    public class ReportesController(ILogger<ReportesController> logger, TicketsDBContext context, IValidator<ReporteRequest> validator, ReportService rpservice, IOptions<TempPathSettings> tempPathOptions, TicketsMediaDBContext mediaContext, DocumentosService docService, IOptions<GoogleMapsSettings> googleMapsOptions) : Controller
    {
        private readonly ILogger<ReportesController> logger = logger;
        private readonly TicketsDBContext ticketsDbContext = context;
        private readonly TicketsMediaDBContext mediaContext = mediaContext;
        private readonly IValidator<ReporteRequest> reportValidator = validator;
        private readonly ReportService reportService = rpservice;
        private readonly DocumentosService documentosService = docService;
        private readonly TempPathSettings tempPathSettings = tempPathOptions.Value;
        private readonly GoogleMapsSettings googleMapsSettings = googleMapsOptions.Value;
        private const int PageSize = 10;


        [HttpGet]
        public IActionResult Index([FromQuery] ReportesFiltroViewModel filtros)
        {
            CargarCatalogos();
            ViewBag.Filters = filtros;

            // save the filters for used after
            GuardarFiltrosEnTempData(filtros);

            // * retrive the data
            var reportes = this.reportService.ObtenerReportes(
                tipoEntrada: filtros.TipoEntrada,
                tipoReporte: filtros.TipoReporte,
                estatusId: filtros.Estatus,
                oficina: filtros.Oficina
            );

            // * pass the googleMapSettings
            ViewBag.GoogleMapsSettings = this.googleMapsSettings;

            return View(reportes);
        }

        [HttpGet("nuevo-reporte")]
        public IActionResult NuevoReporte(int page = 1)
        {
            var totalItems = ticketsDbContext.OprReportes.Count();
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / PageSize);
            ViewBag.CurrentPage = page;

            // * pass the googleMapSettings
            ViewBag.GoogleMapsSettings = this.googleMapsSettings;

            return View();
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
            long folioDetReporte = 0;
            // * almacenar el reporte
            try
            {
                // * almacenar reporte
                (folioReporte, folioDetReporte) = await this.reportService.AlmacenarReporteInicial(request);
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
            List<Guid> _documentosAlmacenados = new();
            if (uploadedFiles?.Any() == true)
            {
                foreach (var fileMetadata in uploadedFiles)
                {
                    try
                    {
                        var documentoId = await documentosService.AlmacenarDocumento(fileMetadata, folioReporte, folioDetReporte, request.IdGenero);
                        _documentosAlmacenados.Add(documentoId);
                    }
                    catch (Exception ex)
                    {
                        logger.LogError(ex, "Error al registrar el documento");
                    }
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
            var folioReporte = this.ProcesarFolioArgs(folioReporteArg, out IActionResult? actionResult);
            if (actionResult != null)
            {
                return actionResult;
            }

            ConstruirUrlRegreso();

            CargarCatalogos();

            // * pass the googleMapSettings
            ViewBag.GoogleMapsSettings = this.googleMapsSettings;

            try
            {
                MostrarReporteViewModel viewModel = new();

                // obtener reporte
                viewModel.Reporte = await this.reportService.ObtenerReportePorFolio(folioReporte);

                // obtener entradas del reporte
                viewModel.Entradas = await this.reportService.ObtenerEntradasReporte(folioReporte);

                // * obtener los documentos adjuntos
                viewModel.Archivos = this.documentosService.ObtenerArchivosAdjuntos(folioReporte);

                return View(viewModel);
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
            var folioReporte = this.ProcesarFolioArgs(folioReporteArg, out IActionResult? actionResult);
            if (actionResult != null)
            {
                return actionResult;
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
                var errors = new Dictionary<string, string>() { { "file", "El archivo adjunto excede el tamaño máximo permitido de 10MB." } };
                return UnprocessableEntity(new
                {
                    errors = errors.Select(e => new
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

        [HttpGet("/api/reportes/ultimos-reportes-hash")]
        public ActionResult<string> GetLastReportsHash()
        {
            try
            {
                var ultimosReportes = ticketsDbContext.OprReportes
                    .OrderByDescending(r => r.FechaRegistro)
                    .Include(r => r.IdEstatusNavigation)
                    .Include(r => r.IdGeneroNavigation)
                    .Take(PageSize)
                    .ToList();
                string hashValue = GenerarReportesHash(ultimosReportes);
                return Ok(hashValue);
            }
            catch (Exception e)
            {
                this.logger.LogError(e, "Error al generar el hash");
                return Conflict(new
                {
                    Title = "Error al generar el hash",
                    Message = e.Message
                });
            }
        }

        [HttpPatch("/api/reportes/{folioReporteArg}")]
        public async Task<IActionResult> ActualizarReporte([FromRoute] string folioReporteArg, ActualizarReporteRequest request)
        {
            var folioReporte = this.ProcesarFolioArgs(folioReporteArg, out IActionResult? actionResult);
            if (actionResult != null)
            {
                this.logger.LogWarning("Error al actualizar el reporte, el formato del folio no es valido");
                return BadRequest(new
                {
                    Title = "El formato del folio no es valido"
                });
            }

            try
            {
                await this.reportService.ActualizarReporte(folioReporte, request);
                this.logger.LogInformation("Reporte con folio {FolioReporte} actualizado correctamente.", folioReporte);
                return Ok(new
                {
                    Title = "Reporte actualizado"
                });
            }
            catch (System.Exception err)
            {
                this.logger.LogError(err, "Error al actualizar el reporte: {message}", err.Message);
                return Conflict(new
                {
                    Title = "Error al actualizar el reporte",
                    err.Message
                });
            }
        }

        [HttpPost("{folioReporteArg}/asignar-oficina")]
        public async Task<IActionResult> AsignarOficina([FromRoute] string folioReporteArg, [FromForm] int idOficina, [FromForm] string comentarios)
        {
            var folioReporte = this.ProcesarFolioArgs(folioReporteArg, out IActionResult? actionResult);
            if (actionResult != null)
            {
                this.logger.LogWarning("Error a asignar oficina al reporte, el formato del folio no es valido.");
                return BadRequest(new
                {
                    Title = "El formato del folio no es valido"
                });
            }

            try
            {
                await this.reportService.AsignarOficina(folioReporte, idOficina, comentarios);
                return Ok(new
                {
                    Title = $"Oficina asignada al reporte '{folioReporte}'"
                });
            }
            catch (KeyNotFoundException kex)
            {
                return BadRequest(new
                {
                    Title = "Error al asignar la oficina al reporte",
                    kex.Message
                });
            }
            catch (Exception ex)
            {
                return Conflict(new
                {
                    Title = "Error al asignar la oficina al reporte",
                    ex.Message
                });
            }
        }

        #region PartialViews
        [HttpGet("partial-view/menu")]
        public IActionResult NuevoReporteMenuPartialView()
        {
            try
            {
                var elements = this.ticketsDbContext.CatReportes.Where(item => item.IdReporte > 0).ToList();
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
                var _tipoReporte = this.ticketsDbContext.CatReportes
                    .FirstOrDefault(item => item.IdReporte == tipoReporteId) ?? throw new ArgumentException($"No se encontró el tipo de reporte con Id {tipoReporteId}");

                // * cargar catalog the estatus
                var estatusList = this.ticketsDbContext.CatEstatuses
                    .OrderBy(e => e.Descripcion)
                    .Select(e => new SelectListItem
                    {
                        Value = e.IdEstatus.ToString(),
                        Text = e.Descripcion
                    }).ToList();
                ViewBag.EstatusList = estatusList;

                // * cargar catalog tipo entradas
                var tipoEntradaList = this.ticketsDbContext.CatTipoEntrada
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
                var reportes = ticketsDbContext.OprReportes
                    .OrderByDescending(r => r.FechaRegistro)
                    .Take(PageSize)
                    .Select(rep => new ReporteDTO
                    {
                        Folio = rep.Folio,
                        Nombre = rep.Nombre,
                        Celular = rep.Celular,
                        Correo = rep.Correo,
                        Telefono = rep.Telefono,
                        Calle = rep.Calle,
                        EntreCalles = rep.EntreCalles,
                        Colonia = rep.Colonia,
                        Localidad = rep.Localidad,
                        Municipio = rep.Municipio,
                        GpsLat = rep.GpsLat,
                        GpsLon = rep.GpsLat,
                        FechaRegistro = rep.FechaRegistro!.Value,
                        IdTipoReporte = rep.IdReporte,
                        TiporReporteDesc = rep.IdReporteNavigation == null ? null : rep.IdReporteNavigation.Descripcion,
                        IdGenero = rep.IdGenero,
                        UsuarioGenero = rep.IdGeneroNavigation,
                        IdEstatus = rep.IdEstatus,
                        EstatusDesc = rep.IdEstatusNavigation == null ? null : rep.IdEstatusNavigation.Descripcion,
                        IdTipoentrada = rep.IdTipoentrada,
                        TipoEntradaDesc = rep.IdTipoentradaNavigation == null ? null : rep.IdTipoentradaNavigation.Descripcion,
                        TotalEntradas = rep.OprDetReportes.Count
                    })
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

        private string GenerarReportesHash(IEnumerable<OprReporte> reportes)
        {
            var _reportesStrings = reportes.Select(item => $"{item.IdReporte}|{item.IdEstatus}").ToList();
            string payload = string.Join("|", _reportesStrings);
            return Convert.ToHexString(System.Security.Cryptography.MD5.HashData(Encoding.UTF8.GetBytes(payload)));
        }

        private void CargarCatalogos()
        {
            // * obtener catalogo tipo-reportes
            var tiposEntrada = this.ticketsDbContext.CatTipoEntrada
                .Where(e => e.IdTipoentrada > 0)
                .OrderBy(e => e.Descripcion)
                .Select(e => new SelectListItem
                {
                    Value = e.IdTipoentrada.ToString(),
                    Text = e.Descripcion
                })
                .ToList();
            ViewBag.TipoEntradaList = tiposEntrada;

            // * obtener catalogo tipo-reportes
            var tiposReportes = this.ticketsDbContext.CatReportes
            .Where(e => e.IdReporte > 0)
                .OrderBy(e => e.Descripcion)
                .Select(e => new SelectListItem
                {
                    Value = e.IdReporte.ToString(),
                    Text = e.Descripcion
                })
                .ToList();
            ViewBag.TiposReporteList = tiposReportes;

            // * obtener catalogo de estatus
            var estatusList = this.ticketsDbContext.CatEstatuses
                .Where(e => e.IdEstatus > 0)
                .OrderBy(e => e.Descripcion)
                .Select(e => new SelectListItem
                {
                    Value = e.IdEstatus.ToString(),
                    Text = e.Descripcion
                }).ToList();
            ViewBag.EstatusList = estatusList;

            // * obtener catalogo de oficinas
            var oficinas = this.ticketsDbContext.CatOficinas
                .Where(e => e.Inactivo != true)
                .OrderBy(e => e.Oficina)
                .Select(ofi => new SelectListItem
                {
                    Text = ofi.Oficina,
                    Value = ofi.Id.ToString()
                })
                .ToList();
            ViewBag.OficinasList = oficinas;
        }

        private void ConstruirUrlRegreso()
        {
            var queryParams = new List<string>();

            void AddParam(string key, string tempDataKey)
            {
                var value = TempData[tempDataKey]?.ToString();
                if (!string.IsNullOrWhiteSpace(value))
                {
                    queryParams.Add($"{key}={Uri.EscapeDataString(value)}");
                }
            }

            var filtros = RecuperarFiltrosDeTempData();
            if (filtros != null)
            {
                AddParam("te", filtros.TipoEntrada.ToString());
                AddParam("tr", filtros.TipoReporte.ToString());
                AddParam("e", filtros.Estatus.ToString());
                AddParam("o", filtros.Oficina.ToString());
            }

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : string.Empty;
            ViewBag.UrlBack = Url.Action("Index", "Reportes") + queryString;
        }

        /// <summary>
        ///  Guarda el filtro en TempData serializado como JSON
        /// </summary>
        private void GuardarFiltrosEnTempData(ReportesFiltroViewModel filtros)
        {
            TempData["reportes-filters"] = System.Text.Json.JsonSerializer.Serialize(filtros);
        }

        /// <summary>
        ///  Recupera el filtro desde TempData deserializando el JSON
        /// </summary>
        private ReportesFiltroViewModel? RecuperarFiltrosDeTempData()
        {
            var filtrosJson = TempData["reportes-filters"] as string;

            return filtrosJson != null
                    ? System.Text.Json.JsonSerializer.Deserialize<ReportesFiltroViewModel>(filtrosJson)
                    : null;
        }

        private long ProcesarFolioArgs(string folioArgs, out IActionResult? actionResult)
        {
            actionResult = null;

            var folioReporte = long.TryParse(folioArgs, out long f) ? f : -1;
            if (folioReporte == -1)
            {
                ViewBag.ErrorTitle = "Folio invalido";
                ViewBag.ErrorMessage = "El formato del folio no es valido";
                actionResult = View("NotFound");
            }
            return folioReporte;
        }

        #endregion
    }
}
