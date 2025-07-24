using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class CatEstatus
{
    public int IdEstatus { get; set; }

    public string? Descripcion { get; set; }

    public string? Tabla { get; set; }

    public bool Inactivo { get; set; }

    public virtual ICollection<OprDetReporte> OprDetReportes { get; set; } = new List<OprDetReporte>();

    public virtual ICollection<OprReporte> OprReportes { get; set; } = new List<OprReporte>();
}
