using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace GimnasioApi.Models;

public partial class GimnasioContext : DbContext
{
    public GimnasioContext(DbContextOptions<GimnasioContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Membresia> Membresias { get; set; }

    public virtual DbSet<Socio> Socios { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Membresia>(entity =>
        {
            entity.HasIndex(e => e.Nombre, "UQ_Membresias_Nombre").IsUnique();

            entity.Property(e => e.Nombre).HasMaxLength(50);
            entity.Property(e => e.Precio).HasColumnType("decimal(10, 2)");
        });

        modelBuilder.Entity<Socio>(entity =>
        {
            entity.HasIndex(e => e.Dni, "UQ_Socios_Dni").IsUnique();

            entity.Property(e => e.Dni)
                .HasMaxLength(8)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Nombres).HasMaxLength(100);
            entity.Property(e => e.Telefono).HasMaxLength(9);

            entity.HasOne(d => d.Membresia).WithMany(p => p.Socios)
                .HasForeignKey(d => d.MembresiaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Socios_Membresias");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
