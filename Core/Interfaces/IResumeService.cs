using System;
using eticket.DTO;

namespace eticket.Core.Interfaces;

public interface IResumeService
{
    public IEnumerable<ReporteResumenEstatusDTO> ObtenerResumenPorEstatus();
    public IEnumerable<ReporteResumenTipoReporteDTO> ObtenerResumenPorTipoReporte();
    public IEnumerable<dynamic> ObtenerResumenPorDias(DateTime fecha1, DateTime fecha2);
}