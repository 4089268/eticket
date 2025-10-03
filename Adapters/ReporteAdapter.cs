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
            FechaRegistro = DateTime.Now,
            IdGenero = request.IdGenero,
            Comentario = request.Observaciones
        };
    }

    public static ReporteDTO ToDTO(this OprReporte oprReporte)
    {
        return new ReporteDTO
        {
            Folio = oprReporte.Folio,
            Nombre = oprReporte.Nombre,
            Celular = oprReporte.Celular,
            Correo = oprReporte.Correo,
            Telefono = oprReporte.Telefono,
            Calle = oprReporte.Calle,
            EntreCalles = oprReporte.EntreCalles,
            Colonia = oprReporte.Colonia,
            Localidad = oprReporte.Localidad,
            Municipio = oprReporte.Municipio,
            GpsLat = oprReporte.GpsLat,
            GpsLon = oprReporte.GpsLon,
            FechaRegistro = DateTime.Now,
            IdEstatus = oprReporte.IdEstatus,
            IdTipoentrada = oprReporte.IdTipoentrada,
            IdTipoReporte = oprReporte.IdReporte,
            IdGenero = oprReporte.IdGenero
        };
    }
}
