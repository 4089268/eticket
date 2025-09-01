using System;

namespace eticket.Core.Interfaces;

public interface IResumeService
{
    public IEnumerable<dynamic> ObtenerResumenPorEstatus();
    public IEnumerable<dynamic> ObtenerResumenPorTipoEntrada();
    public IEnumerable<dynamic> ObtenerResumenPorDias(DateTime fecha1, DateTime fecha2);
}