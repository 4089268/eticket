using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class SysUsuario
{
    public int Id { get; set; }

    public string Usuario { get; set; } = null!;

    public string Contraseña { get; set; } = null!;

    public string Correo { get; set; } = null!;

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public DateTime? FechaCreacion { get; set; }

    public DateTime? UltimoInicioSesion { get; set; }

    public bool? Activo { get; set; }

    public string? Rol { get; set; }
}
