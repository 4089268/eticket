using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class SysUsuario
{
    public int IdUsuario { get; set; }

    public string Usuario { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimoInicioSesion { get; set; }

    public bool? Activo { get; set; }

    public string? Rol { get; set; }

    public string FullName
    {
        get => string.Join(" ", [Nombre, Apellido]);
    }

    public virtual ICollection<OprDetReporte> OprDetReportes { get; set; } = new List<OprDetReporte>();

    public virtual ICollection<OprReporte> OprReportes { get; set; } = new List<OprReporte>();
}
