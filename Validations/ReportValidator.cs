using System;
using System.Data;
using eticket.ViewModels;
using FluentValidation;

namespace eticket.Validations;

public class ReportValidator : AbstractValidator<ReporteRequest>
{
    public ReportValidator()
    {
        RuleFor(req => req.Nombre)
            .NotEmpty()
            .Length(5, 85);

        RuleFor(req => req.Correo)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Correo));

        RuleFor(req => req.Celular)
            .Matches(@"^\d{10,14}$")
            .When(x => !string.IsNullOrEmpty(x.Celular))
            .WithMessage("El telefono debe contener unicamente numeros y un rango de 10-14 digitos");

        RuleFor(req => req.Telefono)
            .Matches(@"^\d{10,14}$")
            .When(x => !string.IsNullOrEmpty(x.Telefono))
            .WithMessage("El telefono debe contener unicamente numeros y un rango de 10-14 digitos");

        RuleFor(req => req.Calle)
            .Length(5, 85);

        RuleFor(req => req.EntreCalles)
            .Length(5, 85);

        RuleFor(req => req.Colonia)
            .Length(5, 65);

        RuleFor(req => req.Localidad)
            .Length(5, 65);

        RuleFor(req => req.Municipio)
            .Length(5, 65);

        RuleFor(req => req.GpsLon)
            .InclusiveBetween(-180, 180)
            .When(x => x.GpsLon.HasValue)
            .WithMessage("La longitud debe estar entre -180 y 180 grados.");

        RuleFor(req => req.GpsLat)
            .InclusiveBetween(-90, 90)
            .When(x => x.GpsLat.HasValue)
            .WithMessage("La latitud debe estar entre -90 y 90 grados.");

        RuleFor(req => req.Observaciones)
            .MaximumLength(400)
            .When(x => !string.IsNullOrEmpty(x.Observaciones));

        RuleFor(req => req.IdReporte)
            .InclusiveBetween(1, 12)
            .WithMessage("El tipo de reporte no es valido");

        RuleFor(req => req.IdTipoEntrada)
            .InclusiveBetween(1, 2)
            .WithMessage("El tipo de entrada no es valido");

    }
}
