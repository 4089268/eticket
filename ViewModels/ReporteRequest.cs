using System;
using System.ComponentModel.DataAnnotations;
using eticket.Models;

namespace eticket.ViewModels;

public class ReporteRequest
{
    public CatReporte TipoReporte { get; set; } = default!;

    public string Nombre { get; set; } = null!;

    public string? Celular { get; set; }

    public string? Correo { get; set; }

    public string? Telefono { get; set; }

    public string? Calle { get; set; }

    [Display(Name = "Entre Calles")]
    public string? EntreCalles { get; set; }

    public string? Colonia { get; set; }

    public string? Localidad { get; set; }

    public string? Municipio { get; set; }

    [Display(Name = "Latitud GPS")]
    public decimal? GpsLat { get; set; }

    [Display(Name = "Longitud GPS")]
    public decimal? GpsLon { get; set; }

    [Display(Name = "Estatus")]
    public int? IdEstatus { get; set; } = 1;

    public string? Observaciones { get; set; }

    [Display(Name = "Tipo de reporte")]
    public int IdTipoReporte { get; set; }

    [Display(Name = "Tipo de entrada")]
    public int IdTipoEntrada { get; set; }

    /// <summary>
    /// Usuario quien genera inicialmente el reporte
    /// </summary>
    public int IdGenero { get; set; }
}