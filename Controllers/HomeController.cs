using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using eticket.Models;
using eticket.DTO;
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

    [HttpGet("/api/entradas/resume-dias")]
    public IActionResult ResumenPorDias()
    {
        try
        {
            // get range of days
            var days = new ResumeDayGraphDTO[30];
            var _targetDay = DateTime.Now.AddDays(-29);
            for (int i = 0; i < days.Count(); i++)
            {
                days[i] = new ResumeDayGraphDTO
                {
                    Date = _targetDay
                };
                _targetDay = _targetDay.AddDays(1);
            }
            
            var now = DateTime.Now.AddMonths(-1);
            var d1 = days[0].Date;
            var d2 = days[29].Date;

            var resumen = this.resumeService.ObtenerResumenPorDias(d1, d2);

            // Process the data
            foreach (var dayData in days)
            {
                dayData.Label = dayData.Date.ToString("MMM dd");
                dayData.Total = resumen.FirstOrDefault(a => a.Dia == dayData.Date)?.TotalEntradas ?? 0;
            }

            return Ok(days);
        }
        catch (Exception err)
        {
            this.logger.LogError(err, "Error al obtener el resumen por dias");
            return Conflict();
        }
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