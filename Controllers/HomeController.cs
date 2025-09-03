using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eticket.Models;
using Microsoft.AspNetCore.Authorization;
using eticket.Core.Interfaces;

namespace eticket.Controllers;

[Authorize]
[Route("/{Controller}")]
public class HomeController(ILogger<HomeController> l, IResumeService rs) : Controller
{
    private readonly ILogger<HomeController> logger = l;
    private readonly IResumeService resumeService = rs;

    [Route("/")]
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }


    #region PartialViews
    [HttpGet("partial-view/status-resume")]
    public IActionResult ResumenEstatusPartialView()
    {
        try
        {
            var resumen = this.resumeService.ObtenerResumenPorEstatus();
            return PartialView("~/Views/Home/Partials/ResumenEstatus.cshtml", resumen);
        }
        catch (Exception err)
        {
            this.logger.LogError(err, "Error al obtener el resumen por estatus");
            ViewData["ErrorTitle"] = "Error al obtener el resumen por estatus";
            ViewData["ErrorMessage"] = err.Message;
            return PartialView("~/Views/Shared/_ErrorAlert.cshtml");
        }
    }

    [HttpGet("partial-view/tipo-reporte-resume")]
    public IActionResult ResumenTipoReportePartialView()
    {
        try
        {
            var resumen = this.resumeService.ObtenerResumenPorTipoReporte();
            return PartialView("~/Views/Home/Partials/ResumenTipo.cshtml", resumen);
        }
        catch (Exception err)
        {
            this.logger.LogError(err, "Error al obtener el resumen por estatus");
            ViewData["ErrorTitle"] = "Error al obtener el resumen por estatus";
            ViewData["ErrorMessage"] = err.Message;
            return PartialView("~/Views/Shared/_ErrorAlert.cshtml");
        }
    }
    #endregion
}
