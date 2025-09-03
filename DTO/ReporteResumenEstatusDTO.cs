using System;

namespace eticket.DTO;

public class ReporteResumenEstatusDTO
{
    public int EstatusId { get; set; }
    public string Estatus { get; set; } = default!;
    public int Total { get; set; }
    public int TotalEntradas { get; set; }
    public IEnumerable<ReporteMinDTO> Reportes { get; set; } = [];
}