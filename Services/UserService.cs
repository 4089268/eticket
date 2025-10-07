using System;
using eticket.Core.Interfaces;
using eticket.Data;
using eticket.DTO;
using eticket.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace eticket.Services;

public class UserService(ILogger<UserService> logger, TicketsDBContext dbContext)
{
    private readonly ILogger<UserService> logger = logger;
    private readonly TicketsDBContext dbContext = dbContext;

    public async Task ActualizarUsuario(EditarUsuarioRequest req)
    {
        // comprobar usuario existe
        var user = this.dbContext.SysUsuarios.Find(req.UsuarioId);
        if (user == null)
        {
            throw new KeyNotFoundException("El usuario no se encuentra registrado en el sistema");
        }

        var transaction = await this.dbContext.Database.BeginTransactionAsync();
        try
        {
            // * actualizar propiedades del usuario
            user.Usuario = req.Usuario!;
            user.Correo = req.Correo!;
            user.Nombre = req.Nombre!;
            user.Apellido = req.Apellido!;
            if (!string.IsNullOrEmpty(req.Contraseña))
            {
                user.Contraseña = BCrypt.Net.BCrypt.HashPassword(req.Contraseña);
            }

            // asignar nivel si se pasa como parametro
            if (req.Nivel != null)
            {
                user.IdNivel = req.Nivel;
            }
            else if (req.Nivel == 0)
            {
                user.IdNivel = null;
            }

            this.dbContext.SysUsuarios.Update(user);
            this.dbContext.SaveChanges();

            // * asignar oficinas al usuario
            if (req.Oficinas?.Any() == true)
            {
                // * remover oficinas anteriores que no pertenescan al usuario
                await this.dbContext.UsuarioOficinas
                    .Where(el => el.IdUsuario == user.IdUsuario && !req.Oficinas.Contains(el.IdOficina))
                    .ExecuteDeleteAsync();

                var oficinasAsignadas = this.dbContext.UsuarioOficinas
                    .Where(el => el.IdUsuario == user.IdUsuario)
                    .Select(el => el.IdOficina)
                    .ToList();

                foreach (var el in req.Oficinas.Where(oficinaId => !oficinasAsignadas.Contains(oficinaId)))
                {
                    this.dbContext.UsuarioOficinas.Add(new()
                    {
                        IdOficina = el,
                        IdUsuario = user.IdUsuario,
                        FechaAsignacion = DateTime.Now
                    });
                }
                this.dbContext.SaveChanges();
            }

            await transaction.CommitAsync();

            this.logger.LogInformation("Usuario {userId} actualizado", user.IdUsuario);
        }
        catch (System.Exception ex)
        {
            await transaction.RollbackAsync();
            this.logger.LogError(ex, "Error al actualizar el usuario");
            throw;
        }
    }

}