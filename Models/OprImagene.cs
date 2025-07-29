using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class OprImagene
{
    public decimal IdImagen { get; set; }

    public long FolioReporte { get; set; }

    public long FolioReporteDetalle { get; set; }

    public byte[]? Imagen { get; set; }

    public string? Descripcion { get; set; }

    public string? IdInsert { get; set; }

    public DateTime? FechaInsert { get; set; }

    public decimal? IdMediatype { get; set; }

    public byte[]? Documento { get; set; }

    public decimal? Filesize { get; set; }

    public string? FileExtension { get; set; }

    public double? Latitud { get; set; }

    public double? Longitud { get; set; }
}
