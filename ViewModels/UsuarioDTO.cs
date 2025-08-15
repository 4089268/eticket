using System;

namespace eticket.ViewModels;

public class UsuarioDTO
{
    public int IdUsuario { get; set; }

    public string Usuario { get; set; } = null!;

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
}
