using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class Notificacione
{
    public int Id { get; set; }

    public string Titulo { get; set; } = null!;

    public string Mensaje { get; set; } = null!;

    public string? LinkUrl { get; set; }

    public string? Tipo { get; set; }

    public DateTime FechaCreacion { get; set; }

    public long? ReporteFolio { get; set; }

    public virtual ICollection<NotificacionesUsuario> NotificacionesUsuarios { get; set; } = new List<NotificacionesUsuario>();

    public virtual OprReporte? ReporteFolioNavigation { get; set; }
}
