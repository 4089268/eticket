using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using eticket.Data;
using eticket.ViewModels;
using FluentValidation;

namespace eticket.Validations;

public class EditUserValidator: AbstractValidator<EditarUsuarioRequest>
{
    private readonly TicketsDBContext _context;

    public EditUserValidator(TicketsDBContext context)
    {
        this._context = context;

        RuleFor(req => req.Usuario)
            .NotEmpty()
            .Length(8, 24)
            .When(x => x.Usuario != "admin")
            .MustAsync(async (request, usuario, cancellation) =>
            {
                // Ignore the record with the same IdUsuario as the one being updated
                return !await _context.SysUsuarios
                    .AnyAsync(u => u.Usuario == usuario && u.IdUsuario != request.UsuarioId, cancellation);
            })
            .WithMessage("El usuario ya se encuentra en uso.");

        RuleFor(req => req.Correo)
            .EmailAddress()
            .When(x => !string.IsNullOrEmpty(x.Correo))
            .MustAsync(async (request, correo, cancellation) =>
            {
                return !await _context.SysUsuarios.AnyAsync(u => u.Correo == correo && u.IdUsuario != request.UsuarioId, cancellation);
            })
            .WithMessage("El correo ya se encuentra en uso.");

        RuleFor(req => req.Nombre)
            .NotEmpty()
            .Length(8, 24);

        RuleFor(req => req.Contraseña)
            .NotEmpty()
            .Length(8, 24)
            .When(x => !string.IsNullOrWhiteSpace(x.Contraseña));

        RuleFor(req => req.ConfirmarContraseña)
            .NotEmpty()
            .Equal(req => req.Contraseña)
            .WithMessage("Las contraseña no coincide.")
            .When(x => !string.IsNullOrWhiteSpace(x.Contraseña));

        RuleFor(req => req.Nivel)
            .MustAsync(async (request, nivel, cancellation) =>
            {
                if (nivel == null)
                {
                    return true;
                }
                return await _context.CatNivelesUsuarios.AnyAsync(n => n.IdNivel == nivel.Value, cancellation);
            })
            .WithMessage("El nivel seleccionado no es valido.");

        RuleFor(req => req.Oficinas)
            .NotEmpty()
            .WithMessage("Debe seleccionar al menos una oficina.")
            .When(req => req.Nivel != null && req.Nivel > 2);
    }
}
