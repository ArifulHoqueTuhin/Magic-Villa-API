using System;
using System.Collections.Generic;
using MagicVillaApi.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MagicVillaApi.Data;

public partial class MagicVillaContext : DbContext
{
    public MagicVillaContext()
    {
    }

    public MagicVillaContext(DbContextOptions<MagicVillaContext> options)
        : base(options)
    {
    }

    //public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } 
    public virtual DbSet<LocalUser> LocalUsers { get; set; }

    public virtual DbSet<VillaList> VillaLists { get; set; }

    public virtual DbSet<VillaNumber> VillaNumbers { get; set; }

    public virtual DbSet<VillaNumberv2> VillaNumberv2s { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
#warning 
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        //base.OnModelCreating(modelBuilder);


        modelBuilder.Entity<LocalUser>(entity =>
        {

            entity.HasKey(e => e.Id).HasName("PK_User");


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
            entity.HasKey(e => e.VillaNo);

            entity.ToTable("Villa Number");

            entity.Property(e => e.VillaNo).HasColumnName("Villa_No");
            entity.Property(e => e.VillaDetails)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .HasColumnName("Villa_Details");
            entity.Property(e => e.VillaId).HasColumnName("Villa_Id");

            entity.HasOne(d => d.Villa).WithMany(p => p.VillaNumbers)
                .HasForeignKey(d => d.VillaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Villa Number_VillaList");
        });

        modelBuilder.Entity<VillaNumberv2>(entity =>
        {
            entity.HasKey(e => new { e.VillaNo, e.VillaId }).HasName("PK_VillaNumber");

            entity.ToTable("VillaNumberv2");

            entity.HasOne(d => d.Villa).WithMany(p => p.VillaNumberv2s)
                .HasForeignKey(d => d.VillaId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_VillaNumber_VillaList");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
