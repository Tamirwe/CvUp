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
        public virtual DbSet<company> companies { get; set; } = null!;
        public virtual DbSet<company_cvs_email> company_cvs_emails { get; set; } = null!;
        public virtual DbSet<company_parser> company_parsers { get; set; } = null!;
        public virtual DbSet<company_stages_type> company_stages_types { get; set; } = null!;
        public virtual DbSet<contact> contacts { get; set; } = null!;
        public virtual DbSet<customer> customers { get; set; } = null!;
        public virtual DbSet<cv> cvs { get; set; } = null!;
        public virtual DbSet<cvs_ascii_sum> cvs_ascii_sums { get; set; } = null!;
        public virtual DbSet<cvs_txt> cvs_txts { get; set; } = null!;
        public virtual DbSet<emails_sent> emails_sents { get; set; } = null!;
        public virtual DbSet<emails_template> emails_templates { get; set; } = null!;
        public virtual DbSet<folder> folders { get; set; } = null!;
        public virtual DbSet<folders_cand> folders_cands { get; set; } = null!;
        public virtual DbSet<parser> parsers { get; set; } = null!;
        public virtual DbSet<parser_rule> parser_rules { get; set; } = null!;
        public virtual DbSet<position> positions { get; set; } = null!;
        public virtual DbSet<position_candidate> position_candidates { get; set; } = null!;
        public virtual DbSet<position_candidate_stage> position_candidate_stages { get; set; } = null!;
        public virtual DbSet<position_contact> position_contacts { get; set; } = null!;
        public virtual DbSet<position_interviewer> position_interviewers { get; set; } = null!;
        public virtual DbSet<registeration_key> registeration_keys { get; set; } = null!;
        public virtual DbSet<stages_type> stages_types { get; set; } = null!;
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

                entity.HasIndex(e => e.last_cv_id, "fk_candidates_last_cv_id_cvs_id");

                entity.HasIndex(e => e.cvdbid, "ix_candidates_cvdbid");

                entity.HasIndex(e => e.last_cv_sent, "ix_candidates_last_cv_sent");

                entity.Property(e => e.adress).HasMaxLength(150);

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.date_updated)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.email).HasMaxLength(150);

                entity.Property(e => e.first_name).HasMaxLength(50);

                entity.Property(e => e.has_duplicates_cvs).HasDefaultValueSql("'0'");

                entity.Property(e => e.last_cv_sent)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.last_name).HasMaxLength(50);

                entity.Property(e => e.name).HasMaxLength(100);

                entity.Property(e => e.phone).HasMaxLength(20);

                entity.Property(e => e.pos_ids).HasColumnType("json");

                entity.Property(e => e.pos_stages).HasColumnType("json");

                entity.Property(e => e.review_html).HasColumnType("blob");

                entity.Property(e => e.review_text).HasMaxLength(14000);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.candidates)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_candidates_company_id_companies_id");
            });

            modelBuilder.Entity<company>(entity =>
            {
                entity.Property(e => e.active_status).HasColumnType("enum('Active','Waite_Complete_Registration','Not_Active')");

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descr).HasMaxLength(500);

                entity.Property(e => e.log_info).HasMaxLength(1500);

                entity.Property(e => e.name).HasMaxLength(150);
            });

            modelBuilder.Entity<company_cvs_email>(entity =>
            {
                entity.ToTable("company_cvs_email");

                entity.HasIndex(e => e.company_id, "fk_company_cvs_email_company_id_companies_id");

                entity.Property(e => e.email).HasMaxLength(150);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.company_cvs_emails)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_company_cvs_email_company_id_companies_id");
            });

            modelBuilder.Entity<company_parser>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_company_parsers_company_id_companies_id");

                entity.HasIndex(e => e.parser_id, "fk_company_parsers_parser_id_parsers_id");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.company_parsers)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_company_parsers_company_id_companies_id");

                entity.HasOne(d => d.parser)
                    .WithMany(p => p.company_parsers)
                    .HasForeignKey(d => d.parser_id)
                    .HasConstraintName("fk_company_parsers_parser_id_parsers_id");
            });

            modelBuilder.Entity<company_stages_type>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_position_candidate_stages_company_id_companies_id");

                entity.Property(e => e.color).HasMaxLength(20);

                entity.Property(e => e.name).HasMaxLength(50);

                entity.Property(e => e.stage_Type).HasMaxLength(50);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.company_stages_types)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_position_candidate_stages_company_id_companies_id");
            });

            modelBuilder.Entity<contact>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_contacts_company_id_companies_id");

                entity.HasIndex(e => e.customer_id, "fk_contacts_customer_id_customers_id");

                entity.Property(e => e.email).HasMaxLength(150);

                entity.Property(e => e.first_name).HasMaxLength(30);

                entity.Property(e => e.last_name).HasMaxLength(30);

                entity.Property(e => e.phone).HasMaxLength(20);

                entity.Property(e => e.role).HasMaxLength(50);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.contacts)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_contacts_company_id_companies_id");

                entity.HasOne(d => d.customer)
                    .WithMany(p => p.contacts)
                    .HasForeignKey(d => d.customer_id)
                    .HasConstraintName("fk_contacts_customer_id_customers_id");
            });

            modelBuilder.Entity<customer>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_customers_company_id_companies_id");

                entity.Property(e => e.address).HasMaxLength(500);

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.descr).HasMaxLength(1000);

                entity.Property(e => e.name).HasMaxLength(100);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.customers)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_customers_company_id_companies_id");
            });

            modelBuilder.Entity<cv>(entity =>
            {
                entity.HasIndex(e => e.candidate_id, "fk_cvs_candidate_id_candidates_id");

                entity.HasIndex(e => e.company_id, "ix_cvs_company_id");

                entity.HasIndex(e => e.cvdbid, "ix_cvs_cvdbid");

                entity.HasIndex(e => e.key_id, "ix_cvs_key_id")
                    .IsUnique();

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.email_id).HasMaxLength(300);

                entity.Property(e => e.from).HasMaxLength(200);

                entity.Property(e => e.key_id).HasMaxLength(30);

                entity.Property(e => e.pos_ids).HasColumnType("json");

                entity.Property(e => e.position).HasMaxLength(300);

                entity.Property(e => e.subject).HasMaxLength(500);

                entity.HasOne(d => d.candidate)
                    .WithMany(p => p.cvs)
                    .HasForeignKey(d => d.candidate_id)
                    .HasConstraintName("fk_cvs_candidate_id_candidates_id");
            });

            modelBuilder.Entity<cvs_ascii_sum>(entity =>
            {
                entity.ToTable("cvs_ascii_sum");

                entity.Property(e => e.cv_folder).HasMaxLength(20);

                entity.Property(e => e.cv_key).HasMaxLength(50);

                entity.Property(e => e.file_extension).HasMaxLength(6);

                entity.Property(e => e.mail_date).HasColumnType("datetime");
            });

            modelBuilder.Entity<cvs_txt>(entity =>
            {
                entity.ToTable("cvs_txt");

                entity.HasIndex(e => e.cv_id, "fk_cvs_txt_cv_id_cvs_id");

                entity.HasIndex(e => e.company_id, "ix_cvs_txt_company_id");

                entity.Property(e => e.cv_txt).HasMaxLength(8000);

                entity.HasOne(d => d.cv)
                    .WithMany(p => p.cvs_txts)
                    .HasForeignKey(d => d.cv_id)
                    .HasConstraintName("fk_cvs_txt_cv_id_cvs_id");
            });

            modelBuilder.Entity<emails_sent>(entity =>
            {
                entity.ToTable("emails_sent");

                entity.HasIndex(e => e.company_id, "fk_emails_sent_company_id_companies_id");

                entity.HasIndex(e => e.user_id, "fk_emails_sent_user_id_users_id");

                entity.Property(e => e.body).HasMaxLength(1500);

                entity.Property(e => e.email_type).HasColumnType("enum('Registration_Approved','Confirm_Registration')");

                entity.Property(e => e.from_address).HasMaxLength(250);

                entity.Property(e => e.sent_date).HasColumnType("datetime");

                entity.Property(e => e.subject).HasMaxLength(500);

                entity.Property(e => e.to_address).HasMaxLength(500);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.emails_sents)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_emails_sent_company_id_companies_id");

                entity.HasOne(d => d.user)
                    .WithMany(p => p.emails_sents)
                    .HasForeignKey(d => d.user_id)
                    .HasConstraintName("fk_emails_sent_user_id_users_id");
            });

            modelBuilder.Entity<emails_template>(entity =>
            {
                entity.Property(e => e.body).HasMaxLength(2000);

                entity.Property(e => e.name).HasMaxLength(50);

                entity.Property(e => e.subject).HasMaxLength(300);
            });

            modelBuilder.Entity<folder>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_folders_company_id_companies_id");

                entity.Property(e => e.name).HasMaxLength(200);

                entity.HasOne(d => d.company)
                    .WithMany(p => p.folders)
                    .HasForeignKey(d => d.company_id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_folders_company_id_companies_id");
            });

            modelBuilder.Entity<folders_cand>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_company_id_folders_cands_companies_id");

                entity.HasIndex(e => e.candidate_id, "fk_folders_cands_candidate_id_candidates_id");

                entity.HasIndex(e => e.folder_id, "fk_folders_cands_folder_id_folders_id");

                entity.HasOne(d => d.candidate)
                    .WithMany(p => p.folders_cands)
                    .HasForeignKey(d => d.candidate_id)
                    .HasConstraintName("fk_folders_cands_candidate_id_candidates_id");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.folders_cands)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_company_id_folders_cands_companies_id");

                entity.HasOne(d => d.folder)
                    .WithMany(p => p.folders_cands)
                    .HasForeignKey(d => d.folder_id)
                    .HasConstraintName("fk_folders_cands_folder_id_folders_id");
            });

            modelBuilder.Entity<parser>(entity =>
            {
                entity.Property(e => e.name).HasMaxLength(250);
            });

            modelBuilder.Entity<parser_rule>(entity =>
            {
                entity.HasIndex(e => e.parser_id, "fk_parser_rules_parser_id_parsers_id");

                entity.Property(e => e.delimiter).HasMaxLength(50);

                entity.Property(e => e.value_type).HasColumnType("enum('Name','Position','Address','CompanyType')");

                entity.HasOne(d => d.parser)
                    .WithMany(p => p.parser_rules)
                    .HasForeignKey(d => d.parser_id)
                    .HasConstraintName("fk_parser_rules_parser_id_parsers_id");
            });

            modelBuilder.Entity<position>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_positions_company_id_companies_id");

                entity.HasIndex(e => e.contact_id, "fk_positions_contact_id_contacts_id");

                entity.HasIndex(e => e.customer_id, "fk_positions_department_id_departments_id");

                entity.HasIndex(e => e.opener_id, "fk_positions_opener_id_users_id");

                entity.HasIndex(e => e.updater_id, "fk_positions_updater_id_users_id");

                entity.Property(e => e.customer_pos_num).HasMaxLength(50);

                entity.Property(e => e.date_created).HasColumnType("datetime");

                entity.Property(e => e.date_updated).HasColumnType("datetime");

                entity.Property(e => e.descr).HasMaxLength(2000);

                entity.Property(e => e.name).HasMaxLength(500);

                entity.Property(e => e.remarks).HasMaxLength(500);

                entity.Property(e => e.requirements).HasMaxLength(2000);

                entity.Property(e => e.status).HasColumnType("enum('Active','Not_Active','Completed')");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.positions)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_positions_company_id_companies_id");

                entity.HasOne(d => d.contact)
                    .WithMany(p => p.positions)
                    .HasForeignKey(d => d.contact_id)
                    .HasConstraintName("fk_positions_contact_id_contacts_id");

                entity.HasOne(d => d.customer)
                    .WithMany(p => p.positions)
                    .HasForeignKey(d => d.customer_id)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_positions_customer_id_customers_id");

                entity.HasOne(d => d.opener)
                    .WithMany(p => p.positionopeners)
                    .HasForeignKey(d => d.opener_id)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_positions_opener_id_users_id");

                entity.HasOne(d => d.updater)
                    .WithMany(p => p.positionupdaters)
                    .HasForeignKey(d => d.updater_id)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("fk_positions_updater_id_users_id");
            });

            modelBuilder.Entity<position_candidate>(entity =>
            {
                entity.HasIndex(e => e.candidate_id, "fk_position_candidates_candidate_id_candidates_id");

                entity.HasIndex(e => e.company_id, "fk_position_candidates_company_id_companies_id");

                entity.HasIndex(e => e.cv_id, "fk_position_candidates_cv_id_cvs_id");

                entity.HasIndex(e => e.position_id, "fk_position_candidates_position_id_positions_id");

                entity.HasIndex(e => e.stage_id, "fk_position_candidates_stage_id_position_candidate_stages_id");

                entity.Property(e => e.cand_cvs).HasColumnType("json");

                entity.Property(e => e.customer_review).HasMaxLength(1000);

                entity.Property(e => e.date_accepted).HasColumnType("datetime");

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.date_cv_sent_to_customer).HasColumnType("datetime");

                entity.Property(e => e.date_interview_customer_request).HasColumnType("datetime");

                entity.Property(e => e.date_msg_accept_reject_sent).HasColumnType("datetime");

                entity.Property(e => e.date_rejected).HasColumnType("datetime");

                entity.Property(e => e.date_remove_candidacy).HasColumnType("datetime");

                entity.Property(e => e.date_sent_talk_request).HasColumnType("datetime");

                entity.Property(e => e.date_updated).HasColumnType("datetime");

                entity.Property(e => e.stage_date).HasColumnType("datetime");

                entity.Property(e => e.stage_type).HasMaxLength(50);

                entity.HasOne(d => d.candidate)
                    .WithMany(p => p.position_candidates)
                    .HasForeignKey(d => d.candidate_id)
                    .HasConstraintName("fk_position_candidates_candidate_id_candidates_id");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.position_candidates)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_position_candidates_company_id_companies_id");

                entity.HasOne(d => d.cv)
                    .WithMany(p => p.position_candidates)
                    .HasForeignKey(d => d.cv_id)
                    .HasConstraintName("fk_position_candidates_cv_id_cvs_id");

                entity.HasOne(d => d.position)
                    .WithMany(p => p.position_candidates)
                    .HasForeignKey(d => d.position_id)
                    .HasConstraintName("fk_position_candidates_position_id_positions_id");
            });

            modelBuilder.Entity<position_candidate_stage>(entity =>
            {
                entity.HasIndex(e => e.candidate_id, "position_candidate_stages_candidate_id_candidates_id");

                entity.HasIndex(e => e.company_id, "position_candidate_stages_company_id_companies_id");

                entity.HasIndex(e => e.position_candidate_id, "position_candidate_stages_pos_candidate_id_pos_candidates_id");

                entity.Property(e => e.stage_date).HasColumnType("datetime");

                entity.Property(e => e.stage_type).HasMaxLength(50);

                entity.HasOne(d => d.candidate)
                    .WithMany(p => p.position_candidate_stages)
                    .HasForeignKey(d => d.candidate_id)
                    .HasConstraintName("position_candidate_stages_candidate_id_candidates_id");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.position_candidate_stages)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("position_candidate_stages_company_id_companies_id");

                entity.HasOne(d => d.position_candidate)
                    .WithMany(p => p.position_candidate_stages)
                    .HasForeignKey(d => d.position_candidate_id)
                    .HasConstraintName("position_candidate_stages_pos_candidate_id_pos_candidates_id");
            });

            modelBuilder.Entity<position_contact>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_position_contacts_company_id_companies_id");

                entity.HasIndex(e => e.contact_id, "fk_position_contacts_contact_id_contacts_id");

                entity.HasIndex(e => e.position_id, "fk_position_contacts_position_id_positions_id");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.position_contacts)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_position_contacts_company_id_companies_id");

                entity.HasOne(d => d.contact)
                    .WithMany(p => p.position_contacts)
                    .HasForeignKey(d => d.contact_id)
                    .HasConstraintName("fk_position_contacts_contact_id_contacts_id");

                entity.HasOne(d => d.position)
                    .WithMany(p => p.position_contacts)
                    .HasForeignKey(d => d.position_id)
                    .HasConstraintName("fk_position_contacts_position_id_positions_id");
            });

            modelBuilder.Entity<position_interviewer>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_position_interviewers_company_id_companies_id");

                entity.HasIndex(e => e.user_id, "fk_position_interviewers_user_id_users_id");

                entity.HasIndex(e => new { e.position_id, e.user_id, e.company_id }, "position_interviewers_position_id_user_id_company_id")
                    .IsUnique();

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.position_interviewers)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_position_interviewers_company_id_companies_id");

                entity.HasOne(d => d.position)
                    .WithMany(p => p.position_interviewers)
                    .HasForeignKey(d => d.position_id)
                    .HasConstraintName("fk_position_interviewers_position_id_positions_id");

                entity.HasOne(d => d.user)
                    .WithMany(p => p.position_interviewers)
                    .HasForeignKey(d => d.user_id)
                    .HasConstraintName("fk_position_interviewers_user_id_users_id");
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

            modelBuilder.Entity<stages_type>(entity =>
            {
                entity.Property(e => e.color).HasMaxLength(20);

                entity.Property(e => e.name).HasMaxLength(50);

                entity.Property(e => e.stage_type).HasMaxLength(50);
            });

            modelBuilder.Entity<user>(entity =>
            {
                entity.HasIndex(e => e.company_id, "fk_users_company_id_companies_id");

                entity.HasIndex(e => new { e.email, e.passwaord }, "uq_users_email_password")
                    .IsUnique();

                entity.Property(e => e.active_status).HasColumnType("enum('Active','Not_Active','Waite_Complete_Registration')");

                entity.Property(e => e.date_created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.email).HasMaxLength(250);

                entity.Property(e => e.first_name).HasMaxLength(20);

                entity.Property(e => e.last_name).HasMaxLength(20);

                entity.Property(e => e.log_info).HasMaxLength(1500);

                entity.Property(e => e.passwaord).HasMaxLength(120);

                entity.Property(e => e.permission_type).HasColumnType("enum('Admin','User')");

                entity.Property(e => e.phone).HasMaxLength(20);

                entity.Property(e => e.refresh_token).HasMaxLength(100);

                entity.Property(e => e.refresh_token_expiry).HasColumnType("datetime");

                entity.HasOne(d => d.company)
                    .WithMany(p => p.users)
                    .HasForeignKey(d => d.company_id)
                    .HasConstraintName("fk_users_company_id_companies_id");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
