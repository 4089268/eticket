using System;
using Microsoft.Extensions.DependencyInjection.Extensions;
using FluentValidation;
using eticket.ViewModels;

namespace eticket.Validations;

public static class ValidationsServiceCollection
{
    public static void AddValidations(this IServiceCollection services)
    {
        services.AddScoped<IValidator<ReporteRequest>, ReportValidator>();
        services.AddScoped<IValidator<UsuarioRequest>, NewUserValidator>();
    }
}
