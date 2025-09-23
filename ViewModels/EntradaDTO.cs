using System;
using Microsoft.Identity.Client;

namespace eticket.ViewModels;

public class EntradaDTO
{
    public long Id { get; set; }

    public long Folio { get; set; }

    public int IdEstatus { get; set; }
    public string? Estatus { get; set; }

    public int IdOperador { get; set; }

    public string? Operador { get; set; }

    public DateTime Fecha { get; set; }

    public string? Observaciones { get; set; }

    public int TotalDocumentosAdjuntos { get; set; } = 0;

    public int TipoMovimientoId { get; set; } = 1;
    public string TipoMovimientoDesc { get; set; } = "Desconocido";
}
