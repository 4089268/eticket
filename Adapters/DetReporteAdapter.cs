using System;
using eticket.Data;
using eticket.Models;
using eticket.ViewModels;

namespace eticket.Adapters;

public static class DetReporteAdapter
{
    public static OprDetReporte ToEntity(this DetReporteRequest request)
    {
        var oprDetReporte = new OprDetReporte
        {
            Folio = request.Folio,
            IdEstatus = request.IdEstatus,
            IdOperador = request.IdOperador,
            Fecha = DateTime.Now,
            Observaciones = request.Observaciones,
            IdTipoMovimiento = (int) TipoMovimientoEnum.Usuario
        };
        return oprDetReporte;
    }

    public static EntradaDTO ToEntradaDTO(this OprDetReporte entity)
    {
        var entrada = new EntradaDTO()
        {
            Id = entity.Id,
            Folio = entity.Folio,
            IdEstatus = entity.IdEstatus,
            Estatus = entity.IdEstatusNavigation?.Descripcion ?? entity.IdEstatus.ToString(),
            IdOperador = entity.IdOperador,
            Operador = entity.IdOperadorNavigation?.FullName,
            Fecha = entity.Fecha,
            Observaciones = entity.Observaciones
        };

        if (entity.IdTipoMovimientoNavigation != null)
        {
            entrada.TipoMovimientoId = entity.IdTipoMovimiento!.Value;
            entrada.TipoMovimientoDesc = entity.IdTipoMovimientoNavigation?.Descripcion ?? "Desconocido";
        }

        return entrada;
    }

}
