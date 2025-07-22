using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using eticket.Models;

namespace eticket.Data;

public partial class DbInitializer
{
    public static async Task SeedAsync(TicketsDBContext context)
    {
        // Ensure DB is created/migrated
        await context.Database.EnsureCreatedAsync();

        if (!context.SysUsuarios.Any())
        {

            string plainPassword = "admin123";
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(plainPassword);

            context.SysUsuarios.Add(new SysUsuario
            {
                Usuario = "admin",
                Contraseña = hashedPassword,
                Correo = "admin@example.com",
                Nombre = "Admin",
                Apellido = "User",
                FechaCreacion = DateTime.UtcNow,
                UltimoInicioSesion = null,
                Activo = true,
                Rol = "Administrador"
            });

            await context.SaveChangesAsync();
        }
    }
}
