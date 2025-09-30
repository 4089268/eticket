using System;

namespace eticket.ViewModels;

public class ActualizarReporteRequest
{
    public int EstatusId { get; set; }
    public int TipoReporte { get; set; }
    public int OficinaId { get; set; }
    public string? Comentarios { get; set; }
}
