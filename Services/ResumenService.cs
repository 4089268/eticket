using System;
using eticket.Core.Interfaces;
using eticket.Data;

namespace eticket.Services;

public class ResumenService(ILogger<ResumenService> logger, TicketsDBContext dbContext) : IResumeService
{
    private readonly ILogger<ResumenService> logger = logger;
    private readonly TicketsDBContext dbContext = dbContext;

    public IEnumerable<dynamic> ObtenerResumenPorEstatus()
    {
        var reportes = this.dbContext.OprReportes
            .GroupJoin(
                dbContext.OprDetReportes,
                repo => repo.Folio,
                det => det.Folio,
                (reporte, detalles) => new { reporte, detalles }
            )
            .GroupBy(item => item.reporte.IdEstatus)
            .Select(group => new
            {
                EstatusId = group.Key,
                Estatus = group.First().reporte.IdEstatusNavigation!.Descripcion,
                Total = group.Count(),
                TotalEntradas = group.Sum(rep => rep.detalles.Count()),
                Reportes = group.Select(rep => new
                {
                    Folio = rep.reporte.Folio,
                    Fecha = rep.reporte.FechaRegistro,
                    Estatus = rep.reporte.IdEstatusNavigation!.Descripcion,
                    TipoEntrada = rep.reporte.IdTipoentradaNavigation!.Descripcion,
                    TipoReporte = rep.reporte.IdReporteNavigation!.Descripcion,
                    UsuarioGenero = rep.reporte.IdEstatusNavigation!.Descripcion,
                })
            })
            .ToArray();
        return reportes;
    }

    public IEnumerable<dynamic> ObtenerResumenPorTipoReporte()
    {
        var reportes = this.dbContext.OprReportes
            .GroupJoin(
                dbContext.OprDetReportes,
                repo => repo.Folio,
                det => det.Folio,
                (reporte, detalles) => new { reporte, detalles }
            )
            .GroupBy(item => item.reporte.IdReporte)
            .Select(group => new
            {
                TipoReporteId = group.Key,
                TipoReporte = group.First().reporte.IdReporteNavigation!.Descripcion,
                Total = group.Count(),
                TotalEntradas = group.Sum(rep => rep.detalles.Count()),
                Reportes = group.Select(rep => new
                {
                    Folio = rep.reporte.Folio,
                    Fecha = rep.reporte.FechaRegistro,
                    Estatus = rep.reporte.IdEstatusNavigation!.Descripcion,
                    TipoEntrada = rep.reporte.IdTipoentradaNavigation!.Descripcion,
                    TipoReporte = rep.reporte.IdReporteNavigation!.Descripcion,
                    UsuarioGenero = rep.reporte.IdEstatusNavigation!.Descripcion,
                })
            })
            .ToArray();
        return reportes;
    }
    
    public IEnumerable<dynamic> ObtenerResumenPorDias(DateTime fecha1, DateTime fecha2)
    {
        var reportes = this.dbContext.OprReportes
            .Where(rep => rep.FechaRegistro >= fecha1 && rep.FechaRegistro <= fecha2 )
            .GroupJoin(
                dbContext.OprDetReportes,
                repo => repo.Folio,
                det => det.Folio,
                (reporte, detalles)=> new { reporte, detalles}
            )
            .GroupBy(item => item.reporte.FechaRegistro!.Value.Date)
            .Select(group => new
            {
                Dia = group.Key,
                Total = group.Count(),
                TotalEntradas = group.Sum(rep => rep.detalles.Count()),
                Reportes = group.Select(rep => new
                {
                    Folio = rep.reporte.Folio,
                    Fecha = rep.reporte.FechaRegistro,
                    Estatus = rep.reporte.IdEstatusNavigation!.Descripcion,
                    TipoEntrada = rep.reporte.IdTipoentradaNavigation!.Descripcion,
                    TipoReporte = rep.reporte.IdReporteNavigation!.Descripcion,
                    UsuarioGenero = rep.reporte.IdEstatusNavigation!.Descripcion,
                })
            })
            .ToArray();
        return reportes;
    }
}