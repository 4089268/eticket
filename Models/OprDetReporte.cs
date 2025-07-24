using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class OprDetReporte
{
    public long Id { get; set; }

    public long Folio { get; set; }

    public int IdEstatus { get; set; }

    public int IdOperador { get; set; }

    public DateTime Fecha { get; set; }

    public string? Observaciones { get; set; }

    public virtual OprReporte FolioNavigation { get; set; } = null!;

    public virtual CatEstatus IdEstatusNavigation { get; set; } = null!;

    public virtual SysUsuario IdOperadorNavigation { get; set; } = null!;
}
