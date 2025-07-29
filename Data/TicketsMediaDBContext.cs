using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using eticket.Models;

namespace eticket.Data;

public partial class TicketsMediaDBContext : DbContext
{
    public TicketsMediaDBContext()
    {
    }

    public TicketsMediaDBContext(DbContextOptions<TicketsMediaDBContext> options)
        : base(options)
    {
    }

    public virtual DbSet<OprImagene> OprImagenes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlServer("Name=TicketDBMedia");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<OprImagene>(entity =>
        {
            entity.HasKey(e => e.IdImagen);

            entity.ToTable("Opr_Imagenes", "Global");

            entity.Property(e => e.IdImagen)
                .ValueGeneratedOnAdd()
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("id_imagen");
            entity.Property(e => e.Descripcion)
                .IsUnicode(false)
                .HasColumnName("descripcion");
            entity.Property(e => e.Documento).HasColumnName("documento");
            entity.Property(e => e.FechaInsert)
                .HasColumnType("datetime")
                .HasColumnName("fecha_insert");
            entity.Property(e => e.FileExtension)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("file_extension");
            entity.Property(e => e.Filesize)
                .HasColumnType("numeric(10, 0)")
                .HasColumnName("filesize");
            entity.Property(e => e.FolioReporte).HasColumnName("folio_reporte");
            entity.Property(e => e.FolioReporteDetalle).HasColumnName("folio_reporte_detalle");
            entity.Property(e => e.IdInsert)
                .HasMaxLength(5)
                .IsUnicode(false)
                .HasColumnName("id_insert");
            entity.Property(e => e.IdMediatype)
                .HasColumnType("numeric(2, 0)")
                .HasColumnName("id_mediatype");
            entity.Property(e => e.Imagen).HasColumnName("imagen");
            entity.Property(e => e.Latitud).HasColumnName("latitud");
            entity.Property(e => e.Longitud).HasColumnName("longitud");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
