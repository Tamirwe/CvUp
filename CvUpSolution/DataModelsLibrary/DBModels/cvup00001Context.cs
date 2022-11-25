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

        public virtual DbSet<candidate> candidates { get; set; } = null!;
        public virtual DbSet<candidate_position_stage> candidate_position_stages { get; set; } = null!;
        public virtual DbSet<company> companies { get; set; } = null!;
        public virtual DbSet<cv> cvs { get; set; } = null!;
        public virtual DbSet<cvs_incremental> cvs_incrementals { get; set; } = null!;
        public virtual DbSet<emails_sent> emails_sents { get; set; } = null!;
        public virtual DbSet<emails_template> emails_templates { get; set; } = null!;
        public virtual DbSet<enum_company_activate_status> enum_company_activate_statuses { get; set; } = null!;
        public virtual DbSet<enum_email_type> enum_email_types { get; set; } = null!;
        public virtual DbSet<enum_lung> enum_lungs { get; set; } = null!;
        public virtual DbSet<enum_position_status> enum_position_statuses { get; set; } = null!;
        public virtual DbSet<enum_role> enum_roles { get; set; } = null!;
        public virtual DbSet<enum_user_activate_status> enum_user_activate_statuses { get; set; } = null!;
        public virtual DbSet<position> positions { get; set; } = null!;
        public virtual DbSet<position_cv> position_cvs { get; set; } = null!;
        public virtual DbSet<positions_incremental> positions_incrementals { get; set; } = null!;
        public virtual DbSet<registeration_key> registeration_keys { get; set; } = null!;
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

            modelBuilder.Entity<candidate>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_candidates_company_id_companies_id");

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.date_updated).HasColumnType("datetime");

                entity.Property(e => e.email).HasMaxLength(150);

                entity.Property(e => e.name).HasMaxLength(100);

                entity.Property(e => e.opinion).HasMaxLength(5000);

                entity.Property(e => e.phone).HasMaxLength(20);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.candidates)
                    .HasForeignKey(d => d.company_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_candidates_company_id_companies_id");
            });

            modelBuilder.Entity<candidate_position_stage>(entity =>
            {
                entity.ToTable("candidate_position_stage");

                entity.HasIndex(e => e.company_id, "fk_position_stage_company_id_companies_id");

                entity.Property(e => e.name).HasMaxLength(50);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.candidate_position_stages)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_position_stage_company_id_companies_id");
            });

            modelBuilder.Entity<company>(entity =>
            {
                entity.HasIndex(e => e.activate_status_id, "fk_companies_activate_status_id_enum_company_activate_status_id");

                entity.HasIndex(e => e.key_email, "uq_companies_key_email")
                    .IsUnique();

                entity.Property(e => e.cvs_email).HasMaxLength(200);

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descr).HasMaxLength(500);

                entity.Property(e => e.key_email).HasMaxLength(15);

                entity.Property(e => e.log_info).HasMaxLength(1500);

                entity.Property(e => e.name).HasMaxLength(150);

                entity.HasOne(d => d.activate_status)
                    .WithMany(p => p.companies)
                    .HasForeignKey(d => d.activate_status_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_companies_activate_status_id_enum_company_activate_status_id");
            });

            modelBuilder.Entity<cv>(entity =>
            {
                entity.HasIndex(e => e.candidate_id, "fk_cvs_candidate_id_candidates_id");

                entity.HasIndex(e => e.company_id, "fk_cvs_stage_company_id_companies_id");

                entity.Property(e => e.id).HasMaxLength(30);

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.email_id).HasMaxLength(300);

                entity.Property(e => e.from).HasMaxLength(200);

                entity.Property(e => e.subject).HasMaxLength(500);

                entity.HasOne(d => d.candidate)
                    .WithMany(p => p.cvs)
                    .HasForeignKey(d => d.candidate_id)
                    .HasConstraintName("fk_cvs_candidate_id_candidates_id");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.cvs)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_cvs_stage_company_id_companies_id");
            });

            modelBuilder.Entity<cvs_incremental>(entity =>
            {
                entity.ToTable("cvs_incremental");

                entity.Property(e => e.name).HasMaxLength(2);
            });

            modelBuilder.Entity<emails_sent>(entity =>
            {
                entity.ToTable("emails_sent");

                entity.HasIndex(e => e.email_type, "fk_emails_sent_email_type_enum_email_type_id");

                entity.HasIndex(e => e.user_id, "fk_emails_sent_user_id_users_id");

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

            modelBuilder.Entity<enum_position_status>(entity =>
            {
                entity.ToTable("enum_position_status");

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

            modelBuilder.Entity<position>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_positions_company_id_companies_id");

                entity.HasIndex(e => e.status_id, "fk_positions_status_id_enum_position_status_id");

                entity.Property(e => e.date_created).HasColumnType("datetime");

                entity.Property(e => e.date_updated).HasColumnType("datetime");

                entity.Property(e => e.descr).HasMaxLength(2000);

                entity.Property(e => e.name).HasMaxLength(500);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.positions)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_positions_company_id_companies_id");

                entity.HasOne(d => d.status)
                    .WithMany(p => p.positions)
                    .HasForeignKey(d => d.status_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_positions_status_id_enum_position_status_id");
            });

            modelBuilder.Entity<position_cv>(entity =>
            {
                entity.HasIndex(e => e.candidate_stage_id, "fk_position_cvs_candidate_stage_id_candidate_position_stage_id");

                entity.HasIndex(e => e.company_id, "fk_position_cvs_company_id_companies_id");

                entity.HasIndex(e => e.cv_id, "fk_position_cvs_cv_id_cvs_id");

                entity.HasIndex(e => e.position_id, "fk_position_cvs_position_id_positions_id");

                entity.Property(e => e.cv_id).HasMaxLength(30);

                entity.HasOne(d => d.candidate_stage)
                    .WithMany(p => p.position_cvs)
                    .HasForeignKey(d => d.candidate_stage_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_position_cvs_candidate_stage_id_candidate_position_stage_id");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.position_cvs)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_position_cvs_company_id_companies_id");

                entity.HasOne(d => d.cv)
                    .WithMany(p => p.position_cvs)
                    .HasForeignKey(d => d.cv_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_position_cvs_cv_id_cvs_id");

                entity.HasOne(d => d.position)
                    .WithMany(p => p.position_cvs)
                    .HasForeignKey(d => d.position_id)
                    .HasConstraintName("fk_position_cvs_position_id_positions_id");
            });

            modelBuilder.Entity<positions_incremental>(entity =>
            {
                entity.ToTable("positions_incremental");

                entity.Property(e => e.name).HasMaxLength(2);
            });

            modelBuilder.Entity<registeration_key>(entity =>
            {
                entity.ToTable("registeration_key");

                entity.Property(e => e.id).HasMaxLength(100);

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.email).HasMaxLength(250);
            });

            modelBuilder.Entity<user>(entity =>
            {
                entity.HasIndex(e => e.activate_status_id, "fk_users_activate_status_id_enum_user_activate_status_id");

                entity.HasIndex(e => e.company_id, "fk_users_company_id_companies_id");

                entity.HasIndex(e => e.role, "fk_users_role_enum_roles_id");

                entity.HasIndex(e => new { e.email, e.passwaord }, "uq_users_email_password")
                    .IsUnique();

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.email).HasMaxLength(250);

                entity.Property(e => e.first_name).HasMaxLength(20);

                entity.Property(e => e.last_name).HasMaxLength(20);

                entity.Property(e => e.log_info).HasMaxLength(1500);

                entity.Property(e => e.passwaord).HasMaxLength(120);

                entity.Property(e => e.refresh_token).HasMaxLength(100);

                entity.Property(e => e.refresh_token_expiry).HasColumnType("datetime");

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
