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
        public virtual DbSet<emails_sent> emails_sents { get; set; } = null!;
        public virtual DbSet<emails_template> emails_templates { get; set; } = null!;
        public virtual DbSet<enum_company_activate_status> enum_company_activate_statuses { get; set; } = null!;
        public virtual DbSet<enum_email_type> enum_email_types { get; set; } = null!;
        public virtual DbSet<enum_lung> enum_lungs { get; set; } = null!;
        public virtual DbSet<enum_role> enum_roles { get; set; } = null!;
        public virtual DbSet<enum_user_activate_status> enum_user_activate_statuses { get; set; } = null!;
        public virtual DbSet<user> users { get; set; } = null!;

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
                entity.HasIndex(e => e.activate_status_id, "fk_companies_activate_status_id_enum_company_activate_status_id");

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descr).HasMaxLength(500);

                entity.Property(e => e.log_info).HasMaxLength(1500);

                entity.Property(e => e.name).HasMaxLength(150);

                entity.HasOne(d => d.activate_status)
                    .WithMany(p => p.companies)
                    .HasForeignKey(d => d.activate_status_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_companies_activate_status_id_enum_company_activate_status_id");
            });

            modelBuilder.Entity<emails_sent>(entity =>
            {
                entity.ToTable("emails_sent");

                entity.HasIndex(e => e.email_type, "fk_emails_sent_email_type_enum_email_type_id");

                entity.HasIndex(e => e.user_id, "fk_emails_sent_user_id_users_id");

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.body).HasMaxLength(1500);

                entity.Property(e => e.from_address).HasMaxLength(250);

                entity.Property(e => e.sent_date).HasColumnType("datetime");

                entity.Property(e => e.subject).HasMaxLength(500);

                entity.Property(e => e.to_address).HasMaxLength(500);

                entity.HasOne(d => d.email_typeNavigation)
                    .WithMany(p => p.emails_sents)
                    .HasForeignKey(d => d.email_type)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_emails_sent_email_type_enum_email_type_id");

                entity.HasOne(d => d.user)
                    .WithMany(p => p.emails_sents)
                    .HasForeignKey(d => d.user_id)
                    .HasConstraintName("fk_emails_sent_user_id_users_id");
            });

            modelBuilder.Entity<emails_template>(entity =>
            {
                entity.HasIndex(e => e.lang, "fk_emails_templates_lang_enum_lung_id");

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.body).HasMaxLength(2000);

                entity.Property(e => e.name).HasMaxLength(50);

                entity.Property(e => e.subject).HasMaxLength(300);

                entity.HasOne(d => d.langNavigation)
                    .WithMany(p => p.emails_templates)
                    .HasForeignKey(d => d.lang)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_emails_templates_lang_enum_lung_id");
            });

            modelBuilder.Entity<enum_company_activate_status>(entity =>
            {
                entity.ToTable("enum_company_activate_status");

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasMaxLength(50);
            });

            modelBuilder.Entity<enum_email_type>(entity =>
            {
                entity.ToTable("enum_email_type");

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasMaxLength(50);
            });

            modelBuilder.Entity<enum_lung>(entity =>
            {
                entity.ToTable("enum_lung");

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasMaxLength(50);
            });

            modelBuilder.Entity<enum_role>(entity =>
            {
                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasMaxLength(20);
            });

            modelBuilder.Entity<enum_user_activate_status>(entity =>
            {
                entity.ToTable("enum_user_activate_status");

                entity.Property(e => e.id).ValueGeneratedNever();

                entity.Property(e => e.name).HasMaxLength(50);
            });

            modelBuilder.Entity<user>(entity =>
            {
                entity.HasIndex(e => e.activate_status_id, "fk_users_activate_status_id_enum_user_activate_status_id");

                entity.HasIndex(e => e.role, "fk_users_role_enum_roles_id");

                entity.HasIndex(e => new { e.company_id, e.email }, "uk_users_company_id_email")
                    .IsUnique();

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.email).HasMaxLength(250);

                entity.Property(e => e.first_name).HasMaxLength(20);

                entity.Property(e => e.last_name).HasMaxLength(20);

                entity.Property(e => e.log_info).HasMaxLength(1500);

                entity.Property(e => e.passwaord).HasMaxLength(20);

                entity.HasOne(d => d.activate_status)
                    .WithMany(p => p.users)
                    .HasForeignKey(d => d.activate_status_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_activate_status_id_enum_user_activate_status_id");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.users)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_users_company_id_companies_id");

                entity.HasOne(d => d.roleNavigation)
                    .WithMany(p => p.users)
                    .HasForeignKey(d => d.role)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_users_role_enum_roles_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
