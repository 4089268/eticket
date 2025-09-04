using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class CatTipoMovimiento
{
    public int Id { get; set; }

    public string? Descripcion { get; set; }

    public string? Categoria { get; set; }

    public virtual ICollection<OprDetReporte> OprDetReportes { get; set; } = new List<OprDetReporte>();
}
