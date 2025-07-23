using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class CatReporte
{
    public int IdReporte { get; set; }

    public string? Descripcion { get; set; }

    public bool Inactivo { get; set; }

    public virtual ICollection<OprReporte> OprReportes { get; set; } = new List<OprReporte>();
}
