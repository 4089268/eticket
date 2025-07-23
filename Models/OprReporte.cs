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

    public int? IdReporte { get; set; }

    public int? IdGenero { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public int? IdEstatus { get; set; }

    public int? IdTipoentrada { get; set; }

    public long Folio { get; set; }

    public virtual CatEstatus? IdEstatusNavigation { get; set; }

    public virtual SysUsuario? IdGeneroNavigation { get; set; }

    public virtual CatReporte? IdReporteNavigation { get; set; }

    public virtual CatTipoEntradum? IdTipoentradaNavigation { get; set; }
}
