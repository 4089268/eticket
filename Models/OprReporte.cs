using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class OprReporte
{
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

    public decimal? IdReporte { get; set; }

    public decimal? IdGenero { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public decimal? IdEstatus { get; set; }

    public string? Referencias { get; set; }

    public decimal Folio { get; set; }
}
