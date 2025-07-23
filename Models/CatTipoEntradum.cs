using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class CatTipoEntradum
{
    public int IdTipoentrada { get; set; }

    public string Descripcion { get; set; } = null!;

    public bool Inactivo { get; set; }

    public virtual ICollection<OprReporte> OprReportes { get; set; } = new List<OprReporte>();
}
