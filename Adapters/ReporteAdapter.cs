using System;
using eticket.Models;
using eticket.ViewModels;

namespace eticket.Adapters;

public static class ReporteAdapter
{
    public static OprReporte ToEntity(this ReporteRequest request)
    {
        return new OprReporte
        {
            Nombre = request.Nombre,
            Celular = request.Celular,
            Correo = request.Correo,
            Telefono = request.Telefono,
            Calle = request.Calle,
            EntreCalles = request.EntreCalles,
            Colonia = request.Colonia,
            Localidad = request.Localidad,
            Municipio = request.Municipio,
            GpsLat = request.GpsLat,
            GpsLon = request.GpsLon,
            IdEstatus = request.IdEstatus,
            IdTipoentrada = request.IdTipoEntrada,
            IdReporte = request.IdTipoReporte,
            FechaRegistro = DateTime.UtcNow,
            IdGenero = request.IdGenero
        };
    }
}
