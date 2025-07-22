using System;
using eticket.Models;
using eticket.ViewModels;

namespace eticket.Adapters;

public class ReporteAdapter
{
    public static OprReporte ToEntity(ReporteRequest request)
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
            Referencias = request.Referencias,
            FechaRegistro = DateTime.UtcNow
        };
    }
}
