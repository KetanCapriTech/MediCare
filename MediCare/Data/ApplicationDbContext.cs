using System;
using System.Collections.Generic;
using MediCareApi.Models;
using Microsoft.EntityFrameworkCore;

namespace MediCareApi.Data;

public partial class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Role_pkey");

            entity.ToTable("Role");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('role_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.CreatedOn).HasColumnName("created_on");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.IsDelete).HasColumnName("is_delete");
            entity.Property(e => e.RoleName).HasColumnName("role_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Users_pkey");

            entity.ToTable("User");

            entity.Property(e => e.Id)
                .HasDefaultValueSql("nextval('user_id_seq'::regclass)")
                .HasColumnName("id");
            entity.Property(e => e.Address).HasColumnName("address");
            entity.Property(e => e.CreatedBy).HasColumnName("created_by");
            entity.Property(e => e.CreatedOn).HasColumnName("created_on");
            entity.Property(e => e.Email).HasColumnName("email");
            entity.Property(e => e.FirstName).HasColumnName("first_name");
            entity.Property(e => e.IsActive).HasColumnName("is_active");
            entity.Property(e => e.LastName).HasColumnName("last_name");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");
            entity.Property(e => e.UpdatedBy).HasColumnName("updated_by");
            entity.Property(e => e.UpdatedOn).HasColumnName("updated_on");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
