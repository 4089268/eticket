using System;
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
            Fecha = DateTime.UtcNow,
            Observaciones = request.Observaciones
        };
        return oprDetReporte;
    }

}
