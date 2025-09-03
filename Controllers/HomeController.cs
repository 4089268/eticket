using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using eticket.Models;
using Microsoft.AspNetCore.Authorization;
using eticket.Core.Interfaces;
using System.Data.Entity.Core.Metadata.Edm;

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
            var now = DateTime.Now.AddMonths(-1);
            var totalDays = DateTime.DaysInMonth(now.Year, now.Month);
            var d1 = new DateTime(now.Year, now.Month, 1);
            var d2 = d1.AddMonths(1).AddDays(-1);

            var resumen = this.resumeService.ObtenerResumenPorDias(d1, d2);

            // Process the data
            var days = new ResumeDayDTO[totalDays];
            for (int i = 0; i < totalDays; i++)
            {
                var resumeDay = resumen.FirstOrDefault(item => item.Dia.Day == (i + 1));
                ResumeDayDTO resumeDayDTO = new()
                {
                    Label = $"{(i + 1)} {now.ToString("MMM")}"
                };

                if (resumeDay != null)
                {
                    resumeDayDTO.Total = resumeDay.TotalEntradas;
                }

                days[i] = resumeDayDTO;
            }

            return Ok(days);
        }
        catch (Exception err)
        {
            this.logger.LogError(err, "Error al obtener el resumen por estatus");
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

class ResumeDayDTO
{
    public string Label { get; set; } = "";
    public int Total { get; set; } = 0;
}