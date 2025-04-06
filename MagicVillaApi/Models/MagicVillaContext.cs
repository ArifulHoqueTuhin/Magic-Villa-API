using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Models;

public partial class MagicVillaContext : DbContext
{
    public MagicVillaContext()
    {
    }

    public MagicVillaContext(DbContextOptions<MagicVillaContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<VillaList> VillaLists { get; set; }

    public virtual DbSet<VillaNumber> VillaNumbers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {

        }

    }
#warning 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Roles)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UserName)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<VillaList>(entity =>
        {
            entity.ToTable("VillaList");

            entity.Property(e => e.Amenity)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.CreateDate).HasColumnType("datetime");
            entity.Property(e => e.Details)
                .HasMaxLength(1000)
                .IsUnicode(false);
            entity.Property(e => e.ImageUrl)
                .HasMaxLength(500)
                .IsUnicode(false);
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.UpdateDate).HasColumnType("datetime");
        });

        modelBuilder.Entity<VillaNumber>(entity =>
        {
            entity.HasKey(e => e.VillaId);

            entity.ToTable("Villa Number");

            entity.Property(e => e.VillaId).HasColumnName("Villa_Id");
            entity.Property(e => e.VillaDetails)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("Villa_Details");

            entity.HasOne(d => d.IdNavigation).WithMany(p => p.VillaNumbers)
                .HasForeignKey(d => d.Id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Villa Number_VillaList");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
