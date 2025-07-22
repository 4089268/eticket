using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class Opr_Reporte
{
    public string nombre { get; set; } = null!;

    public string? celular { get; set; }

    public string? correo { get; set; }

    public string? telefono { get; set; }

    public string? calle { get; set; }

    public string? entre_calles { get; set; }

    public string? colonia { get; set; }

    public string? localidad { get; set; }

    public string? municipio { get; set; }

    public decimal? gps_lat { get; set; }

    public decimal? gps_lon { get; set; }

    public decimal? id_reporte { get; set; }

    public decimal? id_genero { get; set; }

    public DateTime? fecha_registro { get; set; }

    public decimal? id_estatus { get; set; }

    public string? referencias { get; set; }

    public decimal folio { get; set; }
}
