using System;
using System.Data.Entity;
using eticket.Adapters;
using eticket.Data;
using eticket.Models;
using eticket.ViewModels;

namespace eticket.Services;

public class ReportService(ILogger<ReportService> logger, TicketsDBContext context)
{
    private readonly ILogger<ReportService> logger = logger;
    private readonly TicketsDBContext context = context;

    /// <summary>
    /// Obtener reporte por folio con sus relaciones
    /// </summary>
    /// <param name="folio"></param>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException">El reporte no existe</exception>
    public async Task<OprReporte> ObtenerReportePorFolio(long folio)
    {
        var reporte = this.context.OprReportes
            .FirstOrDefault(r => r.Folio == folio);

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

        await Task.CompletedTask;
        return reporte;
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

        using (var transaction = await this.context.Database.BeginTransactionAsync())
        {
            try
            {
                this.context.OprReportes.Add(reporte);
                await this.context.SaveChangesAsync();

                var initialDetail = new OprDetReporte
                {
                    Folio = reporte.Folio,
                    IdEstatus = reporte.IdEstatus!.Value,
                    IdOperador = reporte.IdGenero!.Value,
                    Fecha = DateTime.Now,
                    Observaciones = reporteRequest.Observaciones
                };
                this.context.OprDetReportes.Add(initialDetail);
                await this.context.SaveChangesAsync();
                await transaction.CommitAsync();
                logger.LogInformation("Nuevo reporte creado con folio: {folio}", reporte.Folio);
                return reporte.Folio;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Error storing initial report");
                throw;
            }
        }
    }

    public async Task<OprDetReporte> AlmacenarEntradaReporte(DetReporteRequest detReportRequest)
    {
        // * validar folio existe
        var _ = this.context.OprReportes.FirstOrDefault(r => r.Folio == detReportRequest.Folio)
            ?? throw new InvalidOperationException($"No se encontro reporte con folio: {detReportRequest.Folio}.");

        var reporte = this.context.OprDetReportes.Add(detReportRequest.ToEntity());
        await this.context.SaveChangesAsync();

        return reporte.Entity;
    }
}
