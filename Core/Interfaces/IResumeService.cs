using System;

namespace eticket.Core.Interfaces;

public interface IResumeService
{
    public IEnumerable<dynamic> ObtenerResumenPorEstatus();
    public IEnumerable<dynamic> ObtenerResumenPorTipoReporte();
    public IEnumerable<dynamic> ObtenerResumenPorDias(DateTime fecha1, DateTime fecha2);
}