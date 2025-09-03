using System;
using eticket.Models;

namespace eticket.DTO;

public class ReporteMinDTO
{
    public long Folio { get; set; }
    public DateTime Fecha { get; set; }
    public int? IdEstatus { get; set; }
    public string? EstatusDesc { get; set; }
    public int? IdTipoentrada { get; set; }
    public string? TipoEntradaDesc { get; set; }
    public int? IdTipoReporte { get; set; }
    public string? TiporReporteDesc { get; set; }
    public int? UsuarioGeneroId { get; set; }
    public string? UsuarioGeneroName { get; set; }
}
