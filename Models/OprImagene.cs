using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class OprImagene
{
    public Guid IdImagen { get; set; }

    public long FolioReporte { get; set; }

    public long FolioReporteDetalle { get; set; }

    public string? Descripcion { get; set; }

    public string? IdInsert { get; set; }

    public DateTime? FechaInsert { get; set; }

    public int? IdMediatype { get; set; }

    public byte[]? Documento { get; set; }

    public long? Filesize { get; set; }

    public string? FileExtension { get; set; }

    public double? Latitud { get; set; }

    public double? Longitud { get; set; }
}
