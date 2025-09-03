using System;

namespace eticket.DTO;

public class ReporteResumenTipoReporteDTO
{
    public int TipoReporteId { get; set; }
    public string TipoReporte { get; set; } = default!;
    public int Total { get; set; }
    public int TotalEntradas { get; set; }
    public IEnumerable<ReporteMinDTO> Reportes { get; set; } = [];
}