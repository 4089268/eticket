using System;
using System.Data.Entity;
using System.Diagnostics;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using eticket.Adapters;
using eticket.Data;
using eticket.Models;
using eticket.ViewModels;
using Microsoft.Extensions.FileSystemGlobbing.Internal;

namespace eticket.Services;

public class ReportService(ILogger<ReportService> logger, TicketsDBContext context, TicketsMediaDBContext mediaDBContext, IHttpContextAccessor httpContextAccessor)
{
    private readonly ILogger<ReportService> logger = logger;
    private readonly TicketsDBContext context = context;
    private readonly TicketsMediaDBContext mediaDBContext = mediaDBContext;
    private readonly IHttpContextAccessor httpContextAccessor = httpContextAccessor;

    public IEnumerable<ReporteDTO> ObtenerReportes(int tipoEntrada = 0, int tipoReporte = 0, int estatusId = 0, int oficina = 0, bool incluirEliminados = false)
    {
        var reportesQuery = this.context.OprReportes.AsQueryable();
        if (!incluirEliminados)
        {
            reportesQuery = reportesQuery.Where(r => r.FechaEliminacion == null);
        }

        // * retrive the data
        reportesQuery = reportesQuery
            .OrderByDescending(r => r.FechaRegistro)
            .AsQueryable();

        if (tipoEntrada > 0)
        {
            reportesQuery = reportesQuery.Where(el => el.IdTipoentrada == tipoEntrada);
        }

        if (tipoReporte > 0)
        {
            reportesQuery = reportesQuery.Where(el => el.IdReporte == tipoReporte);
        }

        if (estatusId > 0)
        {
            reportesQuery = reportesQuery.Where(el => el.IdEstatus == estatusId);
        }

        if (oficina == -1)
        {
            reportesQuery = reportesQuery.Where(el => el.IdOficinaNavigation == null);
        }
        else if (oficina > 0)
        {
            reportesQuery = reportesQuery.Where(el => el.IdOficina == oficina);
        }

        var reportes = reportesQuery
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
                GpsLon = rep.GpsLon,
                FechaRegistro = rep.FechaRegistro!.Value,
                IdTipoReporte = rep.IdTipoentrada,
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

        return reportes;
    }

    /// <summary>
    /// Obtener reporte por folio con sus relaciones
    /// </summary>
    /// <param name="folio"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">El reporte no existe</exception>
    public async Task<OprReporte> ObtenerReportePorFolio(long folio, bool incluirEliminados = false)
    {
        var query = this.context.OprReportes.AsQueryable();
        if (!incluirEliminados)
        {
            query = query.Where(r => r.FechaEliminacion == null);
        }

        OprReporte? reporte = query.FirstOrDefault(r => r.Folio == folio);
        if (reporte == null)
        {
            throw new InvalidOperationException($"No se encontro reporte con folio: {folio}.");
        }

        // * load the relations manually
        reporte.IdEstatusNavigation = this.context.CatEstatuses.FirstOrDefault(item => item.IdEstatus == reporte.IdEstatus);
        reporte.IdGeneroNavigation = this.context.SysUsuarios.FirstOrDefault(item => item.IdUsuario == reporte.IdGenero);
        reporte.IdReporteNavigation = this.context.CatReportes.FirstOrDefault(item => item.IdReporte == reporte.IdReporte);
        reporte.IdTipoentradaNavigation = this.context.CatTipoEntrada.FirstOrDefault(item => item.IdTipoentrada == reporte.IdTipoentrada);
        reporte.OprDetReportes = this.context.OprDetReportes.Where(dr => dr.Folio == reporte.Folio).ToList();
        if (reporte.IdOficina != null)
        {
            reporte.IdOficinaNavigation = this.context.CatOficinas.Find(reporte.IdOficina);
        }

        await Task.CompletedTask;
        return reporte;
    }

    public async Task<IEnumerable<EntradaDTO>> ObtenerEntradasReporte(long folio)
    {
        await Task.CompletedTask;
        var reporte = this.context.OprReportes.FirstOrDefault(item => item.Folio == folio) ?? throw new KeyNotFoundException($"Reporte con folio {folio} no encontrado.");

        var entradas = this.context.OprDetReportes
            .Where(item => item.Folio == reporte.Folio)
            .Include(e => e.IdEstatusNavigation)
            .Include(e => e.IdOperadorNavigation)
            .Include(e => e.IdTipoMovimientoNavigation)
            .Select(e => new EntradaDTO
            {
                Id = e.Id,
                Folio = e.Folio,
                IdEstatus = e.IdEstatus,
                Estatus = e.IdEstatusNavigation == null ? "" : e.IdEstatusNavigation.Descripcion,
                IdOperador = e.IdOperador,
                Operador = e.IdOperadorNavigation == null ? "" : e.IdOperadorNavigation.FullName,
                Fecha = e.Fecha,
                Observaciones = e.Observaciones,
                TotalDocumentosAdjuntos = 0,
                TipoMovimientoId = e.IdTipoMovimiento == null ? 0 : e.IdTipoMovimiento.Value,
                TipoMovimientoDesc = e.IdTipoMovimientoNavigation == null ? "" : e.IdTipoMovimientoNavigation.Descripcion!
            })
            .ToList();

        foreach (var entrada in entradas)
        {
            entrada.TotalDocumentosAdjuntos = this.mediaDBContext.OprImagenes.Where(e => e.FolioReporte == entrada.Folio).Count();
        }

        return entradas;
    }

    /// <summary>
    /// Almacenar un reporte inicial
    /// </summary>
    /// <param name="reporteRequest"></param>
    /// <returns>folio del reporte</returns>
    public async Task<long> AlmacenarReporteInicial(ReporteRequest reporteRequest)
    {
        var reporte = reporteRequest.ToEntity();
        reporte.FechaRegistro = DateTime.Now;

        try
        {
            this.context.OprReportes.Add(reporte);
            await this.context.SaveChangesAsync();
            logger.LogInformation("Nuevo reporte creado con folio: {folio}", reporte.Folio);
            return reporte.Folio;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error storing initial report");
            throw;
        }
    }

    public async Task<OprDetReporte> AlmacenarEntradaReporte(DetReporteRequest detReportRequest)
    {
        // recuperar reporte
        var reporte = this.context.OprReportes.Where(e => e.FechaEliminacion == null).FirstOrDefault(e => e.Folio == detReportRequest.Folio);
        if (reporte == null)
        {
            throw new KeyNotFoundException($"El reporte con folio {detReportRequest.Folio} no se encuentra registrado en el sistema.");
        }

        var detReporte = this.context.OprDetReportes.Add(detReportRequest.ToEntity());
        await this.context.SaveChangesAsync();

        return detReporte.Entity;
    }

    public async Task ActualizarReporte(long folio, ActualizarReporteRequest reporteRequest)
    {
        // recuperar reporte
        var reporte = this.context.OprReportes.Where(e => e.FechaEliminacion == null).FirstOrDefault(e => e.Folio == folio);
        if (reporte == null)
        {
            throw new KeyNotFoundException($"El reporte con folio {folio} no se encuentra registrado en el sistema.");
        }

        // * retrive the current userId
        var idOperador = RetriveCurrentUserId();

        // * check what are to be changed
        var modificacionEstatus = reporte.IdEstatus != reporteRequest.EstatusId;
        var modificacionGeneral = reporte.IdReporte != reporteRequest.TipoReporte;
        var modificacionOficina = (reporte.IdOficina ?? 0) != reporteRequest.OficinaId;

        if (modificacionOficina)
        {
            await AsignarOficina(folio, reporteRequest.OficinaId, reporteRequest.Comentarios);
        }

        // * start transaction
            using var transaction = await this.context.Database.BeginTransactionAsync();
        try
        {
            // * make a status record 
            if (modificacionEstatus)
            {
                var previousStatus = this.context.CatEstatuses.FirstOrDefault(item => item.IdEstatus == reporte.IdEstatus)?.Descripcion ?? "DESCONOCIDO";
                var newStatus = this.context.CatEstatuses.FirstOrDefault(item => item.IdEstatus == reporteRequest.EstatusId)?.Descripcion ?? "DESCONOCIDO";
                var oprDetReporte = new OprDetReporte
                {
                    Folio = reporte.Folio,
                    IdEstatus = (int)EstatusReporteEnum.ABIERTO,
                    IdOperador = idOperador,
                    Fecha = DateTime.Now,
                    IdTipoMovimiento = (int)TipoMovimientoEnum.CambioDeEstatus,
                    Observaciones = $"Estatus modificado de {previousStatus} a {newStatus}"
                };
                this.context.OprDetReportes.Add(oprDetReporte);
            }

            if (modificacionGeneral)
            {
                var previousReportType = this.context.CatReportes.FirstOrDefault(item => item.IdReporte == reporte.IdReporte)?.Descripcion ?? "DESCONOCIDO";
                var newReportType = this.context.CatReportes.FirstOrDefault(item => item.IdReporte == reporteRequest.TipoReporte)?.Descripcion ?? "DESCONOCIDO";
                var oprDetReporte = new OprDetReporte
                {
                    Folio = reporte.Folio,
                    IdEstatus = (int)EstatusReporteEnum.ABIERTO,
                    IdOperador = idOperador,
                    Fecha = DateTime.Now,
                    Observaciones = $"Tipo de Reporte modificado de '{previousReportType}' a '{newReportType}'",
                    IdTipoMovimiento = (int)TipoMovimientoEnum.Edicion
                };
                this.context.OprDetReportes.Add(oprDetReporte);
            }

            // * update the entity
            reporte.IdEstatus = reporteRequest.EstatusId;
            reporte.IdReporte = reporteRequest.TipoReporte;
            this.context.OprReportes.Update(reporte);

            // * save all changes
            await this.context.SaveChangesAsync();

            // * commit transaction
            await transaction.CommitAsync();
        }
        catch (System.Exception)
        {
            // rollback transaction if anything fails
            await transaction.RollbackAsync();
            throw;
        }
    }

    /// <summary>
    ///  Asigna la oficina al reporte
    /// </summary>
    /// <param name="folio">Folio del reporte</param>
    /// <param name="idOficina">Id de la oficina a asignar</param>
    /// <param name="comentarios">Comentarios sobre la asignacion</param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">El reporte o la Oficina no existen</exception>
    public async Task AsignarOficina(long folio, int idOficina, string? comentarios)
    {
        // recuperar reporte
        var reporte = this.context.OprReportes.Include(e => e.IdOficinaNavigation).Where(e => e.FechaEliminacion == null).FirstOrDefault(e => e.Folio == folio);
        if (reporte == null)
        {
            throw new KeyNotFoundException($"El reporte con folio {folio} no se encuentra registrado en el sistema.");
        }

        // recuperar oficina
        var oficina = await this.context.CatOficinas.FindAsync(idOficina);
        if (oficina == null)
        {
            throw new KeyNotFoundException($"La oficina con id {idOficina} no se encuentra registada en el sistema.");
        }

        var idOperadorActual = RetriveCurrentUserId();

        using var transaction = await this.context.Database.BeginTransactionAsync();
        try
        {
            var _oficinaAnterior = reporte.IdOficinaNavigation?.Oficina;
            var _oficinaNueva = oficina.Oficina;

            var observacionesBuilder = new StringBuilder();
            if (string.IsNullOrEmpty(_oficinaAnterior))
            {
                observacionesBuilder.AppendFormat("<strong>Reporte asignado a la oficina '{0}'</strong>", _oficinaNueva);
            }
            else
            {
                observacionesBuilder.AppendFormat("<strong>Reporte paso de la oficina'{0}' a la oficina '{1}'</strong>", _oficinaAnterior, _oficinaNueva);
            }

            if (!string.IsNullOrEmpty(comentarios))
            {
                observacionesBuilder.Append(comentarios);
            }

            // actualizar la oficina del reporte
            reporte.IdOficina = oficina.Id;
            this.context.OprReportes.Update(reporte);
            this.context.SaveChanges();

            var oprEntrada = new OprDetReporte
            {
                Folio = reporte.Folio,
                IdEstatus = (int)EstatusReporteEnum.ABIERTO,
                IdOperador = idOperadorActual,
                Fecha = DateTime.Now,
                Observaciones = observacionesBuilder.ToString(),
                IdTipoMovimiento = (int)TipoMovimientoEnum.Asignacion
            };
            this.context.OprDetReportes.Add(oprEntrada);
            this.context.SaveChanges();

            await transaction.CommitAsync();
        }
        catch (System.Exception ex)
        {
            this.logger.LogError(ex, "Error al asignar la oficina al reporte: {message}", ex.Message);
            throw;
        }
    }
    
    /// <summary>
    ///  Eliminar el reporte
    /// </summary>
    /// <param name="folio"></param>
    /// <returns></returns>
    /// <exception cref="KeyNotFoundException">El reporte no existen</exception>
    public async Task EliminarReporte(long folio)
    {
        // recuperar reporte
        var reporte = this.context.OprReportes.Include(e => e.IdOficinaNavigation).Where(e => e.FechaEliminacion == null).FirstOrDefault(e => e.Folio == folio);
        if (reporte == null)
        {
            throw new KeyNotFoundException($"El reporte con folio {folio} no se encuentra registrado en el sistema.");
        }

        // recuperar operador actual
        var idOperadorActual = RetriveCurrentUserId();

        using var transaction = await this.context.Database.BeginTransactionAsync();
        try
        {
            // markar reporte como eliminado
            reporte.FechaEliminacion = DateTime.Now;
            this.context.OprReportes.Update(reporte);
            this.context.SaveChanges();

            // agregar registro de eliminacion
            var oprEntrada = new OprDetReporte
            {
                Folio = reporte.Folio,
                IdEstatus = (int)EstatusReporteEnum.ABIERTO,
                IdOperador = idOperadorActual,
                Fecha = DateTime.Now,
                Observaciones = "El reporte se marcó como eliminado.",
                IdTipoMovimiento = (int)TipoMovimientoEnum.Eliminacion
            };
            this.context.OprDetReportes.Add(oprEntrada);
            this.context.SaveChanges();

            await transaction.CommitAsync();
        }
        catch (System.Exception ex)
        {
            this.logger.LogError(ex, "Error al asignar la oficina al reporte: {message}", ex.Message);
            throw;
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
}