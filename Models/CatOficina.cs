using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class CatOficina
{
    public int Id { get; set; }

    public string Oficina { get; set; } = null!;

    public string Servidor { get; set; } = null!;

    public string BaseDatos { get; set; } = null!;

    public string Usuario { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public bool Inactivo { get; set; }

    public bool Visible { get; set; }

    public virtual ICollection<OprReporte> OprReportes { get; set; } = new List<OprReporte>();

    public virtual ICollection<UsuarioOficina> UsuarioOficinas { get; set; } = new List<UsuarioOficina>();
}
