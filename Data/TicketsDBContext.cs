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

    public virtual DbSet<CatEstatus> CatEstatuses { get; set; }

    public virtual DbSet<CatNivelesUsuario> CatNivelesUsuarios { get; set; }

    public virtual DbSet<CatOficina> CatOficinas { get; set; }

    public virtual DbSet<CatReporte> CatReportes { get; set; }

    public virtual DbSet<CatTipoEntradum> CatTipoEntrada { get; set; }

    public virtual DbSet<CatTipoMovimiento> CatTipoMovimientos { get; set; }

    public virtual DbSet<OprDetReporte> OprDetReportes { get; set; }

    public virtual DbSet<OprReporte> OprReportes { get; set; }

    public virtual DbSet<SysUsuario> SysUsuarios { get; set; }

    public virtual DbSet<UsuarioOficina> UsuarioOficinas { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=TicketDB");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CatEstatus>(entity =>
        {
            entity.HasKey(e => e.IdEstatus);

            entity.ToTable("Cat_Estatus", "Global");

            entity.Property(e => e.IdEstatus).HasColumnName("id_estatus");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Inactivo).HasColumnName("inactivo");
            entity.Property(e => e.Tabla)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("tabla");
        });

        modelBuilder.Entity<CatNivelesUsuario>(entity =>
        {
            entity.HasKey(e => e.IdNivel).HasName("PK__Cat_Nive__9CAF1C5332B92CBE");

            entity.ToTable("Cat_NivelesUsuario", "Global");

            entity.Property(e => e.IdNivel).HasColumnName("id_nivel");
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .HasColumnName("nombre");
        });

        modelBuilder.Entity<CatOficina>(entity =>
        {
            entity.ToTable("Cat_Oficinas", "Global");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BaseDatos).HasColumnName("baseDatos");
            entity.Property(e => e.Contraseña).HasColumnName("contraseña");
            entity.Property(e => e.Inactivo).HasColumnName("inactivo");
            entity.Property(e => e.Oficina).HasColumnName("oficina");
            entity.Property(e => e.Servidor).HasColumnName("servidor");
            entity.Property(e => e.Usuario).HasColumnName("usuario");
            entity.Property(e => e.Visible)
                .HasDefaultValue(true)
                .HasColumnName("visible");
        });

        modelBuilder.Entity<CatReporte>(entity =>
        {
            entity.HasKey(e => e.IdReporte);

            entity.ToTable("Cat_Reportes", "Reportes");

            entity.Property(e => e.IdReporte).HasColumnName("id_reporte");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Inactivo).HasColumnName("inactivo");
        });

        modelBuilder.Entity<CatTipoEntradum>(entity =>
        {
            entity.HasKey(e => e.IdTipoentrada);

            entity.ToTable("Cat_TipoEntrada", "Reportes");

            entity.Property(e => e.IdTipoentrada).HasColumnName("id_tipoentrada");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Inactivo).HasColumnName("inactivo");
        });

        modelBuilder.Entity<CatTipoMovimiento>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Cat_Tipo__3213E83F16B0B17A");

            entity.ToTable("Cat_TipoMovimiento", "Global");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Categoria)
                .HasMaxLength(200)
                .IsUnicode(false)
                .HasColumnName("categoria");
            entity.Property(e => e.Descripcion)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("descripcion");
        });

        modelBuilder.Entity<OprDetReporte>(entity =>
        {
            entity.ToTable("Opr_DetReportes", "Reportes");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Fecha)
                .HasColumnType("datetime")
                .HasColumnName("fecha");
            entity.Property(e => e.Folio).HasColumnName("folio");
            entity.Property(e => e.IdEstatus).HasColumnName("id_estatus");
            entity.Property(e => e.IdOperador).HasColumnName("id_operador");
            entity.Property(e => e.IdTipoMovimiento).HasColumnName("id_tipoMovimiento");
            entity.Property(e => e.Observaciones)
                .IsUnicode(false)
                .HasColumnName("observaciones");

            entity.HasOne(d => d.FolioNavigation).WithMany(p => p.OprDetReportes)
                .HasForeignKey(d => d.Folio)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetReportes_Repore");

            entity.HasOne(d => d.IdEstatusNavigation).WithMany(p => p.OprDetReportes)
                .HasForeignKey(d => d.IdEstatus)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetReportes_Estatus");

            entity.HasOne(d => d.IdOperadorNavigation).WithMany(p => p.OprDetReportes)
                .HasForeignKey(d => d.IdOperador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_DetReportes_Usuarios");

            entity.HasOne(d => d.IdTipoMovimientoNavigation).WithMany(p => p.OprDetReportes)
                .HasForeignKey(d => d.IdTipoMovimiento)
                .HasConstraintName("FK_DetReportes_TipoMovimiento");
        });

        modelBuilder.Entity<OprReporte>(entity =>
        {
            entity.HasKey(e => e.Folio);

            entity.ToTable("Opr_Reportes", "Reportes");

            entity.Property(e => e.Folio)
                .HasDefaultValueSql("([Reportes].[Generar_Folio]())")
                .HasColumnName("folio");
            entity.Property(e => e.Calle)
                .HasMaxLength(85)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("calle");
            entity.Property(e => e.Celular)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("celular");
            entity.Property(e => e.Colonia)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("colonia");
            entity.Property(e => e.Correo)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("correo");
            entity.Property(e => e.EntreCalles)
                .HasMaxLength(85)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("entre_calles");
            entity.Property(e => e.FechaRegistro)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("fecha_registro");
            entity.Property(e => e.GpsLat)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(13, 6)")
                .HasColumnName("gps_lat");
            entity.Property(e => e.GpsLon)
                .HasDefaultValue(0m)
                .HasColumnType("numeric(13, 6)")
                .HasColumnName("gps_lon");
            entity.Property(e => e.IdEstatus)
                .HasDefaultValue(1)
                .HasColumnName("id_estatus");
            entity.Property(e => e.IdGenero).HasColumnName("id_genero");
            entity.Property(e => e.IdOficina).HasColumnName("id_oficina");
            entity.Property(e => e.IdReporte)
                .HasDefaultValue(0)
                .HasColumnName("id_reporte");
            entity.Property(e => e.IdTipoentrada)
                .HasDefaultValue(1)
                .HasColumnName("id_tipoentrada");
            entity.Property(e => e.Localidad)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("localidad");
            entity.Property(e => e.Municipio)
                .HasMaxLength(65)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("municipio");
            entity.Property(e => e.Nombre)
                .HasMaxLength(85)
                .IsUnicode(false)
                .HasColumnName("nombre");
            entity.Property(e => e.Telefono)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasDefaultValue("")
                .HasColumnName("telefono");

            entity.HasOne(d => d.IdEstatusNavigation).WithMany(p => p.OprReportes)
                .HasForeignKey(d => d.IdEstatus)
                .HasConstraintName("FK_Opr_Reportes_Cat_Estatus");

            entity.HasOne(d => d.IdGeneroNavigation).WithMany(p => p.OprReportes)
                .HasForeignKey(d => d.IdGenero)
                .HasConstraintName("FK_Opr_Reportes_Sys_Usuarios");

            entity.HasOne(d => d.IdOficinaNavigation).WithMany(p => p.OprReportes)
                .HasForeignKey(d => d.IdOficina)
                .HasConstraintName("FK_Reportes_Oficina");

            entity.HasOne(d => d.IdReporteNavigation).WithMany(p => p.OprReportes)
                .HasForeignKey(d => d.IdReporte)
                .HasConstraintName("FK_Opr_Reportes_Cat_Reportes");

            entity.HasOne(d => d.IdTipoentradaNavigation).WithMany(p => p.OprReportes)
                .HasForeignKey(d => d.IdTipoentrada)
                .HasConstraintName("FK_Opr_Reportes_Cat_TipoEntrada");
        });

        modelBuilder.Entity<SysUsuario>(entity =>
        {
            entity.HasKey(e => e.IdUsuario).HasName("PK__Sys_Usua__3214EC07E44ED9AE");

            entity.ToTable("Sys_Usuarios", "Global");

            entity.HasIndex(e => e.Correo, "UQ__Sys_Usua__60695A19CCEACA44").IsUnique();

            entity.HasIndex(e => e.Usuario, "UQ__Sys_Usua__E3237CF71C8706E8").IsUnique();

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.Activo).HasDefaultValue(true);
            entity.Property(e => e.Apellido).HasMaxLength(50);
            entity.Property(e => e.Contraseña).HasMaxLength(255);
            entity.Property(e => e.Correo).HasMaxLength(100);
            entity.Property(e => e.FechaCreacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.IdNivel).HasColumnName("id_nivel");
            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Rol)
                .HasMaxLength(30)
                .HasDefaultValue("Usuario");
            entity.Property(e => e.UltimoInicioSesion).HasColumnType("datetime");
            entity.Property(e => e.Usuario).HasMaxLength(50);

            entity.HasOne(d => d.IdNivelNavigation).WithMany(p => p.SysUsuarios)
                .HasForeignKey(d => d.IdNivel)
                .HasConstraintName("FK_Usuarios_Nivel");
        });

        modelBuilder.Entity<UsuarioOficina>(entity =>
        {
            entity.HasKey(e => new { e.IdUsuario, e.IdOficina });

            entity.ToTable("Usuario_Oficina", "Global");

            entity.Property(e => e.IdUsuario).HasColumnName("id_usuario");
            entity.Property(e => e.IdOficina).HasColumnName("id_oficina");
            entity.Property(e => e.FechaAsignacion)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");

            entity.HasOne(d => d.IdOficinaNavigation).WithMany(p => p.UsuarioOficinas)
                .HasForeignKey(d => d.IdOficina)
                .HasConstraintName("FK_Usuario_Oficina_Oficina");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioOficinas)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_Usuario_Oficina_Usuario");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
