using System;

namespace eticket.DTO;

public class ArchivoDTO
{
    public Guid IdImagen { get; set; }

    public long FolioReporte { get; set; }

    public long FolioReporteDetalle { get; set; }

    public string? Descripcion { get; set; }

    public DateTime? FechaInsert { get; set; }
    public long Filesize { get; set; }

    public string Mediatype { get; set; } = string.Empty;
    public string FileExtension { get; set; } = string.Empty;

    public string UrlImage
    {
        get => $"/api/document/{IdImagen.ToString()}";
    }

    public string FileSizeRedeable
    {
        get => eticket.Utils.BytesSizeCast.ToHumanReadableSize(Filesize);
    }
    
}
