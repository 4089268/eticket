using System;
using eticket.Models;

namespace eticket.ViewModels;

using Microsoft.AspNetCore.Mvc;

public class ReportesFiltroViewModel
{
    [FromQuery(Name = "te")]
    public int TipoEntrada { get; set; }

    [FromQuery(Name = "tr")]
    public int TipoReporte { get; set; }

    [FromQuery(Name = "e")]
    public int Estatus { get; set; }

    [FromQuery(Name = "o")]
    public int Oficina { get; set; }

    [FromQuery(Name = "q")]
    public string? Search { get; set; }
}
