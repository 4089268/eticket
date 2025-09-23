using System;
using System.ComponentModel.DataAnnotations;
using eticket.Data;

namespace eticket.ViewModels;

public class DetReporteRequest
{
    [Display(Name = "Estatus")]
    public int IdEstatus { get; set; } = (int) EstatusReporteEnum.ABIERTO;

    public long Folio { get; set; }

    public int IdOperador { get; set; }

    public DateTime Fecha { get; set; } = DateTime.UtcNow;

    [Display(Name = "Observaciones")]
    public string? Observaciones { get; set; }

}
