using System;
using eticket.Data;
using eticket.Models;
using eticket.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace eticket.Services;
public class NotificacionService
{
    private readonly ILogger<NotificacionService> logger;
    private readonly TicketsDBContext ticketContext;

    public NotificacionService(ILogger<NotificacionService> l, TicketsDBContext c)
    {
        logger = l;
        ticketContext = c;
    }

    public async Task CrearNotificacionReporte(string titulo, string mensaje, ReporteDTO rep, IEnumerable<int> usuarios)
    {
        var transaction = await this.ticketContext.Database.BeginTransactionAsync();

        try
        {
            var noti = new Notificacione
            {
                Titulo = titulo,
                Mensaje = mensaje,
                LinkUrl = $"/Reportes/{rep.Folio}",
                Tipo = "Reporte",
                FechaCreacion = DateTime.Now,
                ReporteFolio = rep.Folio
            };
            this.ticketContext.Notificaciones.Add(noti);
            await this.ticketContext.SaveChangesAsync();

            var listaUsuarios = this.ticketContext.SysUsuarios.Where(e => usuarios.Contains(e.IdUsuario)).ToList();
            var listaNotificacionesUsuarios = listaUsuarios.Select(usu =>
                new NotificacionesUsuario
                {
                    UsuarioId = usu.IdUsuario,
                    NotificacionId = noti.Id,
                }
            ).ToList();
            await this.ticketContext.NotificacionesUsuarios.AddRangeAsync(listaNotificacionesUsuarios);
            await this.ticketContext.SaveChangesAsync();

            await transaction.CommitAsync();
        }
        catch (System.Exception ex)
        {
            this.logger.LogError(ex, "Error al genearar las notificaciones.");
            await transaction.RollbackAsync();
        }
    }


    public async Task<List<NotificacionesUsuario>> GetUserNotifications(int userId)
    {
        return await ticketContext.NotificacionesUsuarios
            .Include(e => e.Notificacion)
            .Where(nu => nu.UsuarioId == userId)
            .ToListAsync();
    }

}
