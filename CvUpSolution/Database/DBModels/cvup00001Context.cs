using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Database.models
{
    public partial class cvup00001Context : DbContext
    {
        public cvup00001Context()
        {
        }

        public cvup00001Context(DbContextOptions<cvup00001Context> options)
            : base(options)
        {
        }

        public virtual DbSet<company> companies { get; set; } = null!;
        public virtual DbSet<roles_enum> roles_enums { get; set; } = null!;
        public virtual DbSet<user> users { get; set; } = null!;
        public virtual DbSet<users_role> users_roles { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseMySql("server=localhost;port=3306;user=root;password=!Shalot5;database=cvup00001", Microsoft.EntityFrameworkCore.ServerVersion.Parse("8.0.30-mysql"));
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.UseCollation("utf8mb4_general_ci")
                .HasCharSet("utf8mb4");

            modelBuilder.Entity<company>(entity =>
            {
                entity.Property(e => e.name).HasMaxLength(150);
            });

            modelBuilder.Entity<roles_enum>(entity =>
            {
                entity.ToTable("roles_enum");

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasMaxLength(20);
            });

            modelBuilder.Entity<user>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_users_company_id_companies_id");

                entity.Property(e => e.email).HasMaxLength(250);

                entity.Property(e => e.first_name).HasMaxLength(20);

                entity.Property(e => e.last_name).HasMaxLength(20);

                entity.Property(e => e.passwaord).HasMaxLength(20);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.users)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_users_company_id_companies_id");
            });

            modelBuilder.Entity<users_role>(entity =>
            {
                entity.HasIndex(e => e.role_id, "users_roles_role_id_roles_enum_id");

                entity.HasIndex(e => e.user_id, "users_roles_user_id_users_id");

                entity.HasOne(d => d.role)
                    .WithMany(p => p.users_roles)
                    .HasForeignKey(d => d.role_id)
                    .HasConstraintName("users_roles_role_id_roles_enum_id");

                entity.HasOne(d => d.user)
                    .WithMany(p => p.users_roles)
                    .HasForeignKey(d => d.user_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("users_roles_user_id_users_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
