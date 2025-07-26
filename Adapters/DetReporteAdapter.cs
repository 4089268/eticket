using System;
using Azure.Core;
using eticket.Models;
using eticket.ViewModels;

namespace eticket.Adapters;

public class DetReporteAdapter
{

    public static OprDetReporte ToEntity(DetReporteRequest request)
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
