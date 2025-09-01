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
                Entradas = group.Sum(rep => rep.detalles.Count())
            })
            .ToArray();
        return reportes;
    }

    public IEnumerable<dynamic> ObtenerResumenPorTipoEntrada()
    {
        var reportes = this.dbContext.OprReportes
            .GroupJoin(
                dbContext.OprDetReportes,
                repo => repo.Folio,
                det => det.Folio,
                (reporte, detalles) => new { reporte, detalles }
            )
            .GroupBy(item => item.reporte.IdTipoentrada)
            .Select(group => new
            {
                TipoEntradaId = group.Key,
                TipoEntrada = group.First().reporte.IdTipoentradaNavigation!.Descripcion,
                Total = group.Count(),
                Entradas = group.Sum(rep => rep.detalles.Count())
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
                Entradas = group.Sum(rep=> rep.detalles.Count())
            })
            .ToArray();
        return reportes;
    }
}