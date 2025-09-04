using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class CatNivelesUsuario
{
    public int IdNivel { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<SysUsuario> SysUsuarios { get; set; } = new List<SysUsuario>();
}
