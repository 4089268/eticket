using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class UsuarioOficina
{
    public int IdUsuario { get; set; }

    public int IdOficina { get; set; }

    public DateTime? FechaAsignacion { get; set; }

    public virtual CatOficina IdOficinaNavigation { get; set; } = null!;

    public virtual SysUsuario IdUsuarioNavigation { get; set; } = null!;
}
