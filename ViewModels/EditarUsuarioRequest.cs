using System;

namespace eticket.ViewModels;

public class EditarUsuarioRequest
{
    public int UsuarioId { get; set; }
    public string? Usuario { get; set; }

    public string? Correo { get; set; }

    public string? Nombre { get; set; }

    public string? Apellido { get; set; }

    public string? Rol { get; set; }

    public string? Contraseña { get; set; }
    public string? ConfirmarContraseña { get; set; }
}
