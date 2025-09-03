using System;
using eticket.DTO;
using eticket.Models;

namespace eticket.Adapters;

public static class OprImageAdapter
{
    public static ArchivoDTO ToDTO(this OprImagene oprImagen)
    {
        var archivo = new ArchivoDTO
        {
            IdImagen = oprImagen.IdImagen,
            FolioReporte = oprImagen.FolioReporte,
            FolioReporteDetalle = oprImagen.FolioReporteDetalle,
            Descripcion = oprImagen.Descripcion,
            FechaInsert = oprImagen.FechaInsert,
            Filesize = oprImagen.Filesize!.Value,
            Mediatype = string.Empty,
            FileExtension = oprImagen.FileExtension ?? string.Empty
        };
        return archivo;
    }
}
