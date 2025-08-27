using System;
using eticket.Models;

namespace eticket.ViewModels;

public class ReporteDTO
{
    public long Folio { get; set; }

    public string Nombre { get; set; } = null!;

    public string? Celular { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public string? Calle { get; set; }

    public string? EntreCalles { get; set; }

    public string? Colonia { get; set; }

    public string? Localidad { get; set; }

    public string? Municipio { get; set; }

    public decimal? GpsLat { get; set; }

    public decimal? GpsLon { get; set; }
    public DateTime FechaRegistro { get; set; }


    public int? IdTipoReporte { get; set; }
    public string? TiporReporteDesc { get; set; }

    public int? IdGenero { get; set; }
    public SysUsuario? UsuarioGenero { get; set; }

    public int? IdEstatus { get; set; }
    public string? EstatusDesc { get; set; }

    public int? IdTipoentrada { get; set; }
    public string? TipoEntradaDesc { get; set; }


    public int TotalEntradas { get; set; }
}
