using System;
using System.Security.Cryptography;
using eticket.Models;
using eticket.ViewModels;

namespace eticket.Adapters;

public static class UsersAdapter
{
    public static UsuarioDTO ToUserDTO(this SysUsuario sysUsuario)
    {
        var user = new UsuarioDTO()
        {
            IdUsuario = sysUsuario.IdUsuario,
            Usuario = sysUsuario.Usuario,
            Correo = sysUsuario.Correo,
            Nombre = sysUsuario.Nombre,
            Apellido = sysUsuario.Apellido,
            FechaCreacion = sysUsuario.FechaCreacion,
            UltimoInicioSesion = sysUsuario.UltimoInicioSesion,
            Activo = sysUsuario.Activo,
            Rol = sysUsuario.Rol,
        };
        return user;
    }

    public static EditarUsuarioRequest ToUserEditRequest(this SysUsuario sysUsuario)
    {
        var user = new EditarUsuarioRequest()
        {
            UsuarioId = sysUsuario.IdUsuario,
            Usuario = sysUsuario.Usuario,
            Correo = sysUsuario.Correo,
            Nombre = sysUsuario.Nombre,
            Apellido = sysUsuario.Apellido
        };
        return user;
    }

    public static SysUsuario ToEntity(this UsuarioRequest request)
    {
        var sysUsuario = new SysUsuario
        {
            Usuario = request.Usuario!,
            Correo = request.Correo!,
            Nombre = request.Nombre,
            Apellido = request.Apellido,
            FechaCreacion = DateTime.Now,
            Activo = true,
            Rol = request.Rol ?? "Usuario",
            Contraseña = BCrypt.Net.BCrypt.HashPassword(request.Contraseña)
        };
        return sysUsuario;
    }


}
