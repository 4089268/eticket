using System;
using System.Data.Entity;
using eticket.Core.Hubs;
using eticket.Data;
using Microsoft.AspNetCore.SignalR;

namespace eticket.Core.Jobs;

public class NotificacionDispatcher(ILogger<NotificacionDispatcher> l, IServiceScopeFactory sf, IHubContext<TicketsHub> hubContext  ) : BackgroundService
{
    private readonly ILogger<NotificacionDispatcher> _logger = l;
    private readonly IServiceScopeFactory _scopeFactory = sf;
    private readonly IHubContext<TicketsHub> _hubContext = hubContext;
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("📬 Notification dispatcher started.");

        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = _scopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TicketsDBContext>();

            var pending = db.NotificacionesUsuarios
                .Where(n => n.FechaEnvio == null)
                .Take(10)
                .ToList();

            foreach (var n in pending)
            {
                try
                {
                    var noti = await db.Notificaciones.FindAsync(n.NotificacionId);
                    if(noti != null)
                    {
                        await _hubContext.Clients.User(n.UsuarioId.ToString()).SendAsync("ReceiveNotification", noti.Titulo, noti.LinkUrl);
                        n.FechaEnvio = DateTime.Now;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Error enviando notificación {n.Id}");
                }
            }

            await db.SaveChangesAsync(stoppingToken);

            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}
