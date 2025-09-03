using System;
using eticket.DTO;
using eticket.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace eticket.ViewModels;

public class MostrarReporteViewModel
{
    public OprReporte Reporte { get; set; } = default!;
    public IEnumerable<EntradaDTO> Entradas { get; set; } = [];
    public List<SelectListItem> EstatusList { get; set; } = [];
    public IEnumerable<ArchivoDTO> Archivos { get; set; } = [];
}
