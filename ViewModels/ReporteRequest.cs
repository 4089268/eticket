using System;
using System.ComponentModel.DataAnnotations;

namespace eticket.ViewModels;

public class ReporteRequest
{
    [Required]
    [StringLength(85)]
    public string Nombre { get; set; } = null!;

    [StringLength(15)]
    public string? Celular { get; set; }

    [EmailAddress]
    [StringLength(65)]
    public string? Correo { get; set; }

    [StringLength(15)]
    public string? Telefono { get; set; }

    [Required]
    [StringLength(85)]
    public string? Calle { get; set; }

    [StringLength(85)]
    [Display(Name = "Entre Calles")]
    public string? EntreCalles { get; set; }

    [Required]
    [StringLength(85)]
    public string? Colonia { get; set; }

    [Required]
    [StringLength(85)]
    public string? Localidad { get; set; }

    [Required]
    [StringLength(85)]
    public string? Municipio { get; set; }

    [Range(-180, 180)]
    [Display(Name = "Latitud GPS")]
    public decimal? GpsLat { get; set; }

    [Range(-180, 180)]
    [Display(Name = "Longitud GPS")]
    public decimal? GpsLon { get; set; }

    public decimal? IdReporte { get; set; }

    [Display(Name = "Estatus")]
    [Range(1, 10)]
    public int? IdEstatus { get; set; } = 1;

    [StringLength(4000)]
    public string? Observaciones { get; set; }

    [Range(1, 12)]
    [Display(Name = "Tipo de reporte")]
    public int IdTipoReporte { get; set; }

    [Range(1, 12)]
    [Display(Name = "Tipo de entrada")]
    public int IdTipoEntrada { get; set; }
}
