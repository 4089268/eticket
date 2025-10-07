using System;
using System.Security.Claims;
using eticket.Core.Interfaces;
using eticket.Data;
using eticket.DTO;
using eticket.Models;

namespace eticket.Services;

public class ResumenService(ILogger<ResumenService> logger, TicketsDBContext dbContext, IHttpContextAccessor httpContextAccessor) : IResumeService
{
    private readonly ILogger<ResumenService> logger = logger;
    private readonly TicketsDBContext dbContext = dbContext;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;


    public IEnumerable<ReporteResumenEstatusDTO> ObtenerResumenPorEstatus()
    {
        // Filtrar reportes
        var reportesQuery = this.dbContext.OprReportes.Where(el => el.FechaEliminacion == null).AsQueryable();
        FiltrarReportePorNivelUsuario(ref reportesQuery);

        // * get the data
        var reportesRaw = reportesQuery
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
                Estatus = group.First().reporte.IdEstatusNavigation,
                Total = group.Count(),
                TotalEntradas = group.Sum(rep => rep.detalles.Count()),
                Reportes = group.Select(rep => new
                {
                    Folio = rep.reporte.Folio,
                    Fecha = rep.reporte.FechaRegistro,
                    Estatus = rep.reporte.IdEstatusNavigation,
                    TipoEntrada = rep.reporte.IdTipoentradaNavigation,
                    TipoReporte = rep.reporte.IdReporteNavigation,
                    Usuario = rep.reporte.IdGeneroNavigation
                })
            })
            .ToArray();

        // process the data
        var reportes = new List<ReporteResumenEstatusDTO>();
        foreach (var rp in reportesRaw)
        {
            var reporteDTO = new ReporteResumenEstatusDTO
            {
                EstatusId = rp.Estatus?.IdEstatus ?? 0,
                Estatus = rp.Estatus?.Descripcion ?? "Desconocido",
                Total = rp.Total,
                TotalEntradas = rp.TotalEntradas,
                Reportes = rp.Reportes.Select(detRep => new ReporteMinDTO
                {
                    Folio = detRep.Folio,
                    Fecha = detRep.Fecha!.Value,
                    IdEstatus = detRep.Estatus?.IdEstatus ?? 0,
                    EstatusDesc = detRep.Estatus?.Descripcion ?? "Desconocido",
                    IdTipoentrada = detRep.TipoEntrada?.IdTipoentrada ?? 0,
                    TipoEntradaDesc = detRep.TipoEntrada?.Descripcion ?? "Desconocido",
                    IdTipoReporte = detRep.TipoReporte?.IdReporte ?? 0,
                    TiporReporteDesc = detRep.TipoReporte?.Descripcion ?? "Desconocido",
                    UsuarioGeneroId = 0,
                    UsuarioGeneroName = "Desconocido"
                })
            };
            reportes.Add(reporteDTO);
        }
        return reportes;
    }

    public IEnumerable<ReporteResumenTipoReporteDTO> ObtenerResumenPorTipoReporte()
    {
        // Filtrar reportes
        var reportesQuery = this.dbContext.OprReportes.Where(el => el.FechaEliminacion == null).AsQueryable();
        FiltrarReportePorNivelUsuario(ref reportesQuery);

        var reportesRaw = reportesQuery
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
                TipoReporte = group.First().reporte.IdReporteNavigation,
                Total = group.Count(),
                TotalEntradas = group.Sum(rep => rep.detalles.Count()),
                Reportes = group.Select(rep => new
                {
                    Folio = rep.reporte.Folio,
                    Fecha = rep.reporte.FechaRegistro,
                    Estatus = rep.reporte.IdEstatusNavigation,
                    TipoEntrada = rep.reporte.IdTipoentradaNavigation,
                    TipoReporte = rep.reporte.IdReporteNavigation,
                    Usuario = rep.reporte.IdGeneroNavigation
                })
            })
            .ToArray();

        // process the data
        var reportes = new List<ReporteResumenTipoReporteDTO>();
        foreach (var rp in reportesRaw)
        {
            var reporteDTO = new ReporteResumenTipoReporteDTO
            {
                TipoReporteId = rp.TipoReporte?.IdReporte ?? 0,
                TipoReporte = rp.TipoReporte?.Descripcion ?? "Desconocido",
                Total = rp.Total,
                TotalEntradas = rp.TotalEntradas,
                Reportes = rp.Reportes.Select(detRep => new ReporteMinDTO
                {
                    Folio = detRep.Folio,
                    Fecha = detRep.Fecha!.Value,
                    IdEstatus = detRep.Estatus?.IdEstatus ?? 0,
                    EstatusDesc = detRep.Estatus?.Descripcion ?? "Desconocido",
                    IdTipoentrada = detRep.TipoEntrada?.IdTipoentrada ?? 0,
                    TipoEntradaDesc = detRep.TipoEntrada?.Descripcion ?? "Desconocido",
                    IdTipoReporte = detRep.TipoReporte?.IdReporte ?? 0,
                    TiporReporteDesc = detRep.TipoReporte?.Descripcion ?? "Desconocido",
                    UsuarioGeneroId = 0,
                    UsuarioGeneroName = "Desconocido"
                })
            };
            reportes.Add(reporteDTO);
        }

        return reportes;
    }

    public IEnumerable<dynamic> ObtenerResumenPorDias(DateTime fecha1, DateTime fecha2)
    {
        // Filtrar reportes
        var reportesQuery = this.dbContext.OprReportes.Where(el => el.FechaEliminacion == null).AsQueryable();
        FiltrarReportePorNivelUsuario(ref reportesQuery);

        var reportes = reportesQuery
            .Where(rep => rep.FechaRegistro >= fecha1 && rep.FechaRegistro <= fecha2)
            .GroupJoin(
                dbContext.OprDetReportes,
                repo => repo.Folio,
                det => det.Folio,
                (reporte, detalles) => new { reporte, detalles }
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


    /// <summary>
    ///  Comprueba el nivel de usuario y filtra los reportes que no coincidan con las oficinas asignadas.
    /// </summary>
    /// <param name="query"></param>
    private void FiltrarReportePorNivelUsuario(ref IQueryable<OprReporte> query)
    {
        var _nivelUsuario = RetriveCurrentUserLevel();
        if (_nivelUsuario >= (int)NivelesUsuarioEnum.Director_Oficina)
        {
            var _usuarioId = RetriveCurrentUserId();
            query = query
                .Where(rep => dbContext.UsuarioOficinas
                    .Any(uo => uo.IdUsuario == _usuarioId && uo.IdOficina == rep.IdOficina)
                );
        }
    }

    private int RetriveCurrentUserId()
    {
        var userIdClaim = httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
        if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out var userId))
        {
            throw new InvalidOperationException("No se pudo obtener el IdUsuario del contexto HTTP.");
        }
        return userId;
    }

    private int RetriveCurrentUserLevel()
    {
        // * comprobar el nivel del usuario
        var nivelUsuarioClaim = httpContextAccessor.HttpContext?.User.FindFirstValue(CustomClaimTypes.NivelUsuario);
        if (nivelUsuarioClaim == null || !int.TryParse(nivelUsuarioClaim, out var nivelUsuario))
        {
            throw new InvalidOperationException("No se pudo obtener el IdUsuario del contexto HTTP.");
        }
        return nivelUsuario;
    }

}