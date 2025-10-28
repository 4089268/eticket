using System;
using System.Collections.Generic;

namespace eticket.Models;

public partial class NotificacionesUsuario
{
    public int Id { get; set; }

    public int UsuarioId { get; set; }

    public int NotificacionId { get; set; }

    public DateTime? FechaEnvio { get; set; }

    public DateTime? FechaLectura { get; set; }

    public virtual Notificacione Notificacion { get; set; } = null!;

    public virtual SysUsuario Usuario { get; set; } = null!;
}
