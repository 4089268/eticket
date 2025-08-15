using System;
using Microsoft.EntityFrameworkCore;
using eticket.Data;
using eticket.ViewModels;
using FluentValidation;

namespace eticket.Validations;

public class NewUserValidator: AbstractValidator<UsuarioRequest>
{
    private readonly TicketsDBContext _context;

    public NewUserValidator(TicketsDBContext context)
    {
        this._context = context;

        RuleFor(req => req.Usuario)
            .NotEmpty()
            .Length(8, 24)
            .MustAsync(async (usuario, cancellation) =>
            {
                return !await _context.SysUsuarios.AnyAsync(u => u.Usuario == usuario, cancellation);
            })
            .WithMessage("El usuario ya existe.");

        RuleFor(req => req.Correo)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Correo))
            .MustAsync(async (correo, cancellation) =>
            {
                return !await _context.SysUsuarios.AnyAsync(u => u.Correo == correo, cancellation);
            })
            .WithMessage("El correo ya existe.");

        RuleFor(req => req.Nombre)
            .NotEmpty()
            .Length(8, 24);

        RuleFor(req => req.Contraseña)
            .NotEmpty()
            .Length(8, 24);

        RuleFor(req => req.ConfirmarContraseña)
            .NotEmpty()
            .Equal(req => req.Contraseña)
            .WithMessage("Las contraseña no coincide.");
    }
}
