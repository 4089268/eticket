using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using eticket.Models;

namespace eticket.Data;

public partial class TicketsDBContext : DbContext
{
    public TicketsDBContext()
    {
    }

    public TicketsDBContext(DbContextOptions<TicketsDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Opr_Reporte> Opr_Reportes { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=TicketDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Opr_Reporte>(entity =>
        {
            entity.HasKey(e => e.folio);

            entity.ToTable("Opr_Reportes", "Reportes");

            entity.Property(e => e.folio)
                .HasDefaultValueSql("([reportes].[Generar_Folio]())")
                .HasColumnType("numeric(12, 0)");
            entity.Property(e => e.calle)
                .HasMaxLength(85)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.celular)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.colonia)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.correo)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.entre_calles)
                .HasMaxLength(85)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.fecha_registro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.gps_lat)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(13, 6)");
            entity.Property(e => e.gps_lon)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(13, 6)");
            entity.Property(e => e.id_estatus).HasColumnType("numeric(10, 0)");
            entity.Property(e => e.id_genero).HasColumnType("numeric(10, 0)");
            entity.Property(e => e.id_reporte)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(10, 0)");
            entity.Property(e => e.localidad)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.municipio)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.nombre)
                .HasMaxLength(85)
                .IsUnicode(false);
            entity.Property(e => e.referencias)
                .IsUnicode(false)
                .HasDefaultValue("");
            entity.Property(e => e.telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC075AF0E02D");

            entity.HasIndex(e => e.Correo, "IX_Usuarios_Correo");

            entity.HasIndex(e => e.Usuario1, "IX_Usuarios_NombreUsuario");

            entity.HasIndex(e => e.Correo, "UQ__Usuarios__60695A19E0480A9A").IsUnique();

            entity.HasIndex(e => e.Usuario1, "UQ__Usuarios__E3237CF744B3F542").IsUnique();

            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.Contraseña).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Rol)
                .HasMaxLength(30)
                .HasDefaultValue("Usuario");
            entity.Property(e => e.UltimoInicioSesion).HasColumnType("datetime");
            entity.Property(e => e.Usuario1)
                .HasMaxLength(50)
                .HasColumnName("Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
