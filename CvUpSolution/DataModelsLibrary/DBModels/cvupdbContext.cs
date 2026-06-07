using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Database.models;

public partial class cvupdbContext : DbContext
{
    public cvupdbContext()
    {
    }

    public cvupdbContext(DbContextOptions<cvupdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ai_analyze_cv> ai_analyze_cvs { get; set; }

    public virtual DbSet<analyzed_cv> analyzed_cvs { get; set; }

    public virtual DbSet<auth_out_email> auth_out_emails { get; set; }

    public virtual DbSet<black_cand> black_cands { get; set; }

    public virtual DbSet<cand_pos_stage> cand_pos_stages { get; set; }

    public virtual DbSet<candidate> candidates { get; set; }

    public virtual DbSet<company> companies { get; set; }

    public virtual DbSet<company_cvs_email> company_cvs_emails { get; set; }

    public virtual DbSet<company_parser> company_parsers { get; set; }

    public virtual DbSet<contact> contacts { get; set; }

    public virtual DbSet<customer> customers { get; set; }

    public virtual DbSet<cv> cvs { get; set; }

    public virtual DbSet<cvs_txt> cvs_txts { get; set; }

    public virtual DbSet<emails_template> emails_templates { get; set; }

    public virtual DbSet<folder> folders { get; set; }

    public virtual DbSet<folders_cand> folders_cands { get; set; }

    public virtual DbSet<keyword> keywords { get; set; }

    public virtual DbSet<keywords_group> keywords_groups { get; set; }

    public virtual DbSet<parser> parsers { get; set; }

    public virtual DbSet<parser_rule> parser_rules { get; set; }

    public virtual DbSet<position> positions { get; set; }

    public virtual DbSet<position_candidate> position_candidates { get; set; }

    public virtual DbSet<position_contact> position_contacts { get; set; }

    public virtual DbSet<position_interviewer> position_interviewers { get; set; }

    public virtual DbSet<position_type> position_types { get; set; }

    public virtual DbSet<registeration_key> registeration_keys { get; set; }

    public virtual DbSet<search> searches { get; set; }

    public virtual DbSet<sent_email> sent_emails { get; set; }

    public virtual DbSet<user> users { get; set; }

    public virtual DbSet<users_refresh_token> users_refresh_tokens { get; set; }

   
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasPostgresExtension("vector");

        modelBuilder.Entity<ai_analyze_cv>(entity =>
        {
            entity.HasKey(e => e.id).HasName("ai_analyze_cvs_pkey");

            entity.Property(e => e.area).HasMaxLength(20);
            entity.Property(e => e.city).HasMaxLength(50);
            entity.Property(e => e.companies).HasMaxLength(500);
            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.date_updated)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.education).HasMaxLength(500);
            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.jobs_titles_en).HasMaxLength(500);
            entity.Property(e => e.jobs_titles_he).HasMaxLength(500);
            entity.Property(e => e.languages).HasMaxLength(150);
            entity.Property(e => e.military_service).HasMaxLength(250);
            entity.Property(e => e.name).HasMaxLength(101);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.profession_skills_en).HasMaxLength(500);
            entity.Property(e => e.profession_skills_he).HasMaxLength(500);
            entity.Property(e => e.profession_words_en).HasMaxLength(500);
            entity.Property(e => e.profession_words_he).HasMaxLength(500);
            entity.Property(e => e.region).HasMaxLength(20);
            entity.Property(e => e.seniority).HasMaxLength(50);
            entity.Property(e => e.skills).HasMaxLength(600);
            entity.Property(e => e.summary_en).HasMaxLength(1000);
            entity.Property(e => e.summary_he).HasMaxLength(1000);

            entity.HasOne(d => d.candidate).WithMany(p => p.ai_analyze_cvs)
                .HasForeignKey(d => d.candidate_id)
                .HasConstraintName("fk_ai_analyze_cvs_candidate_id_candidates_id");
        });

        modelBuilder.Entity<analyzed_cv>(entity =>
        {
            entity.HasKey(e => e.id).HasName("analyzed_cvs_pkey");

            entity.Property(e => e.area).HasMaxLength(20);
            entity.Property(e => e.city_he).HasMaxLength(50);
            entity.Property(e => e.created_at).HasDefaultValueSql("now()");
            entity.Property(e => e.education).HasColumnType("jsonb");
            entity.Property(e => e.email).HasMaxLength(255);
            entity.Property(e => e.military_service_he).HasMaxLength(255);
            entity.Property(e => e.name).HasMaxLength(255);
            entity.Property(e => e.phone).HasMaxLength(50);
            entity.Property(e => e.profession_words).HasColumnType("jsonb");
            entity.Property(e => e.region).HasMaxLength(20);
            entity.Property(e => e.seniority_en).HasMaxLength(50);
            entity.Property(e => e.seniority_he).HasMaxLength(50);
            entity.Property(e => e.updated_at).HasDefaultValueSql("now()");
            entity.Property(e => e.work_experience).HasColumnType("jsonb");

            entity.HasOne(d => d.candidate).WithMany(p => p.analyzed_cvs)
                .HasForeignKey(d => d.candidate_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("analyzed_cvs_candidate_id_fkey");

            entity.HasOne(d => d.cv).WithMany(p => p.analyzed_cvs)
                .HasForeignKey(d => d.cv_id)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("analyzed_cvs_cv_id_fkey");
        });

        modelBuilder.Entity<auth_out_email>(entity =>
        {
            entity.HasKey(e => e.id).HasName("auth_out_emails_pkey");

            entity.Property(e => e.body).HasMaxLength(1500);
            entity.Property(e => e.from_address).HasMaxLength(250);
            entity.Property(e => e.sent_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.subject).HasMaxLength(500);
            entity.Property(e => e.to_address).HasMaxLength(500);

            entity.HasOne(d => d.company).WithMany(p => p.auth_out_emails)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_auth_out_emails_company_id_companies_id");

            entity.HasOne(d => d.user).WithMany(p => p.auth_out_emails)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("fk_auth_out_emails_user_id_users_id");
        });

        modelBuilder.Entity<black_cand>(entity =>
        {
            entity.HasKey(e => e.id).HasName("black_cands_pkey");

            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.name).HasMaxLength(101);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.remarks).HasMaxLength(160);
            entity.Property(e => e.updated)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
        });

        modelBuilder.Entity<cand_pos_stage>(entity =>
        {
            entity.HasKey(e => e.id).HasName("cand_pos_stages_pkey");

            entity.Property(e => e.color).HasMaxLength(20);
            entity.Property(e => e.name).HasMaxLength(50);
            entity.Property(e => e.stage_Type).HasMaxLength(50);
            entity.Property(e => e.stage_event).HasMaxLength(50);

            entity.HasOne(d => d.company).WithMany(p => p.cand_pos_stages)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_cand_pos_stages_company_id_companies_id");
        });

        modelBuilder.Entity<candidate>(entity =>
        {
            entity.HasKey(e => e.id).HasName("candidates_pkey");

            entity.HasIndex(e => e.cvdbid, "ix_candidates_cvdbid");

            entity.HasIndex(e => e.last_cv_sent, "ix_candidates_last_cv_sent");

            entity.Property(e => e.adress).HasMaxLength(150);
            entity.Property(e => e.city).HasMaxLength(50);
            entity.Property(e => e.customers_reviews).HasColumnType("jsonb");
            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.date_updated)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.first_name).HasMaxLength(50);
            entity.Property(e => e.folders_ids).HasColumnType("jsonb");
            entity.Property(e => e.last_cv_sent)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.last_name).HasMaxLength(50);
            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.pos_ids).HasColumnType("jsonb");
            entity.Property(e => e.pos_stages).HasColumnType("jsonb");
            entity.Property(e => e.review).HasMaxLength(8000);
            entity.Property(e => e.review_date).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.company).WithMany(p => p.candidates)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_candidates_company_id_companies_id");
        });

        modelBuilder.Entity<company>(entity =>
        {
            entity.HasKey(e => e.id).HasName("companies_pkey");

            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.descr).HasMaxLength(500);
            entity.Property(e => e.log_info).HasMaxLength(1500);
            entity.Property(e => e.name).HasMaxLength(150);
        });

        modelBuilder.Entity<company_cvs_email>(entity =>
        {
            entity.HasKey(e => e.id).HasName("company_cvs_email_pkey");

            entity.ToTable("company_cvs_email");

            entity.Property(e => e.email).HasMaxLength(150);

            entity.HasOne(d => d.company).WithMany(p => p.company_cvs_emails)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_company_cvs_email_company_id_companies_id");
        });

        modelBuilder.Entity<company_parser>(entity =>
        {
            entity.HasKey(e => e.id).HasName("company_parsers_pkey");

            entity.HasOne(d => d.company).WithMany(p => p.company_parsers)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_company_parsers_company_id_companies_id");

            entity.HasOne(d => d.parser).WithMany(p => p.company_parsers)
                .HasForeignKey(d => d.parser_id)
                .HasConstraintName("fk_company_parsers_parser_id_parsers_id");
        });

        modelBuilder.Entity<contact>(entity =>
        {
            entity.HasKey(e => e.id).HasName("contacts_pkey");

            entity.Property(e => e.email).HasMaxLength(150);
            entity.Property(e => e.first_name).HasMaxLength(30);
            entity.Property(e => e.last_name).HasMaxLength(30);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.role).HasMaxLength(50);

            entity.HasOne(d => d.company).WithMany(p => p.contacts)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_contacts_company_id_companies_id");

            entity.HasOne(d => d.customer).WithMany(p => p.contacts)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_contacts_customer_id_customers_id");
        });

        modelBuilder.Entity<customer>(entity =>
        {
            entity.HasKey(e => e.id).HasName("customers_pkey");

            entity.Property(e => e.address).HasMaxLength(500);
            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.descr).HasMaxLength(1000);
            entity.Property(e => e.name).HasMaxLength(100);

            entity.HasOne(d => d.company).WithMany(p => p.customers)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_customers_company_id_companies_id");
        });

        modelBuilder.Entity<cv>(entity =>
        {
            entity.HasKey(e => e.id).HasName("cvs_pkey");

            entity.HasIndex(e => e.company_id, "ix_cvs_company_id");

            entity.HasIndex(e => e.cvdbid, "ix_cvs_cvdbid");

            entity.HasIndex(e => e.key_id, "ix_cvs_key_id").IsUnique();

            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email_id).HasMaxLength(300);
            entity.Property(e => e.file_extension).HasMaxLength(6);
            entity.Property(e => e.from).HasMaxLength(200);
            entity.Property(e => e.key_id).HasMaxLength(30);
            entity.Property(e => e.pos_ids).HasColumnType("jsonb");
            entity.Property(e => e.position).HasMaxLength(300);
            entity.Property(e => e.subject).HasMaxLength(500);

            entity.HasOne(d => d.candidate).WithMany(p => p.cvs)
                .HasForeignKey(d => d.candidate_id)
                .HasConstraintName("fk_cvs_candidate_id_candidates_id");

            entity.HasOne(d => d.position_type).WithMany(p => p.cvs)
                .HasForeignKey(d => d.position_type_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_cvs_position_type_id_position_types_id");
        });

        modelBuilder.Entity<cvs_txt>(entity =>
        {
            entity.HasKey(e => e.id).HasName("cvs_txt_pkey");

            entity.ToTable("cvs_txt");

            entity.HasIndex(e => e.company_id, "ix_cvs_txt_company_id");

            entity.Property(e => e.cv_txt).HasMaxLength(8000);
            entity.Property(e => e.email_subject).HasMaxLength(500);

            entity.HasOne(d => d.candidate).WithMany(p => p.cvs_txts)
                .HasForeignKey(d => d.candidate_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_cvs_txt_candidate_id_candidates_id");

            entity.HasOne(d => d.cv).WithMany(p => p.cvs_txts)
                .HasForeignKey(d => d.cv_id)
                .HasConstraintName("fk_cvs_txt_cv_id_cvs_id");
        });

        modelBuilder.Entity<emails_template>(entity =>
        {
            entity.HasKey(e => e.id).HasName("emails_templates_pkey");

            entity.Property(e => e.body).HasMaxLength(2000);
            entity.Property(e => e.name).HasMaxLength(50);
            entity.Property(e => e.stage_to_update).HasMaxLength(50);
            entity.Property(e => e.subject).HasMaxLength(300);
        });

        modelBuilder.Entity<folder>(entity =>
        {
            entity.HasKey(e => e.id).HasName("folders_pkey");

            entity.Property(e => e.name).HasMaxLength(200);

            entity.HasOne(d => d.company).WithMany(p => p.folders)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_folders_company_id_companies_id");
        });

        modelBuilder.Entity<folders_cand>(entity =>
        {
            entity.HasKey(e => e.id).HasName("folders_cands_pkey");

            entity.HasOne(d => d.candidate).WithMany(p => p.folders_cands)
                .HasForeignKey(d => d.candidate_id)
                .HasConstraintName("fk_folders_cands_candidate_id_candidates_id");

            entity.HasOne(d => d.company).WithMany(p => p.folders_cands)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_company_id_folders_cands_companies_id");

            entity.HasOne(d => d.folder).WithMany(p => p.folders_cands)
                .HasForeignKey(d => d.folder_id)
                .HasConstraintName("fk_folders_cands_folder_id_folders_id");
        });

        modelBuilder.Entity<keyword>(entity =>
        {
            entity.HasKey(e => e.id).HasName("keywords_pkey");

            entity.Property(e => e.name_en).HasMaxLength(150);
            entity.Property(e => e.name_he).HasMaxLength(150);
            entity.Property(e => e.updated)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.company).WithMany(p => p.keywords)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("keywords_company_id_companies_id");

            entity.HasOne(d => d.group).WithMany(p => p.keywords)
                .HasForeignKey(d => d.group_id)
                .HasConstraintName("keywords_group_id_keywords_groups_id");
        });

        modelBuilder.Entity<keywords_group>(entity =>
        {
            entity.HasKey(e => e.id).HasName("keywords_groups_pkey");

            entity.Property(e => e.name).HasMaxLength(100);
            entity.Property(e => e.updated)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.company).WithMany(p => p.keywords_groups)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("keywords_groups_company_id_companies_id");
        });

        modelBuilder.Entity<parser>(entity =>
        {
            entity.HasKey(e => e.id).HasName("parsers_pkey");

            entity.Property(e => e.name).HasMaxLength(250);
        });

        modelBuilder.Entity<parser_rule>(entity =>
        {
            entity.HasKey(e => e.id).HasName("parser_rules_pkey");

            entity.Property(e => e.delimiter).HasMaxLength(50);

            entity.HasOne(d => d.parser).WithMany(p => p.parser_rules)
                .HasForeignKey(d => d.parser_id)
                .HasConstraintName("fk_parser_rules_parser_id_parsers_id");
        });

        modelBuilder.Entity<position>(entity =>
        {
            entity.HasKey(e => e.id).HasName("positions_pkey");

            entity.Property(e => e.customer_pos_num).HasMaxLength(50);
            entity.Property(e => e.date_created).HasColumnType("timestamp without time zone");
            entity.Property(e => e.date_updated).HasColumnType("timestamp without time zone");
            entity.Property(e => e.descr).HasMaxLength(6000);
            entity.Property(e => e.match_email_subject).HasMaxLength(250);
            entity.Property(e => e.name).HasMaxLength(500);
            entity.Property(e => e.remarks).HasMaxLength(500);
            entity.Property(e => e.requirements).HasMaxLength(6000);

            entity.HasOne(d => d.company).WithMany(p => p.positions)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_positions_company_id_companies_id");

            entity.HasOne(d => d.contact).WithMany(p => p.positions)
                .HasForeignKey(d => d.contact_id)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_positions_contact_id_contacts_id");

            entity.HasOne(d => d.customer).WithMany(p => p.positions)
                .HasForeignKey(d => d.customer_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_positions_customer_id_customers_id");
        });

        modelBuilder.Entity<position_candidate>(entity =>
        {
            entity.HasKey(e => e.id).HasName("position_candidates_pkey");

            entity.Property(e => e.accepted).HasColumnType("timestamp without time zone");
            entity.Property(e => e.call_email_to_candidate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.cand_cvs).HasColumnType("jsonb");
            entity.Property(e => e.customer_interview).HasColumnType("timestamp without time zone");
            entity.Property(e => e.customer_review).HasMaxLength(1000);
            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.date_cv_sent_to_customer_tmp).HasColumnType("timestamp without time zone");
            entity.Property(e => e.date_msg_accept_reject_sent_tmp).HasColumnType("timestamp without time zone");
            entity.Property(e => e.date_sent_talk_request_tmp).HasColumnType("timestamp without time zone");
            entity.Property(e => e.date_updated).HasColumnType("timestamp without time zone");
            entity.Property(e => e.email_to_contact).HasColumnType("timestamp without time zone");
            entity.Property(e => e.reject_email_to_candidate).HasColumnType("timestamp without time zone");
            entity.Property(e => e.rejected).HasColumnType("timestamp without time zone");
            entity.Property(e => e.remove_candidacy).HasColumnType("timestamp without time zone");
            entity.Property(e => e.stage_date).HasColumnType("timestamp without time zone");
            entity.Property(e => e.stage_type).HasMaxLength(50);

            entity.HasOne(d => d.candidate).WithMany(p => p.position_candidates)
                .HasForeignKey(d => d.candidate_id)
                .HasConstraintName("fk_position_candidates_candidate_id_candidates_id");

            entity.HasOne(d => d.company).WithMany(p => p.position_candidates)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_position_candidates_company_id_companies_id");

            entity.HasOne(d => d.cv).WithMany(p => p.position_candidates)
                .HasForeignKey(d => d.cv_id)
                .HasConstraintName("fk_position_candidates_cv_id_cvs_id");

            entity.HasOne(d => d.position).WithMany(p => p.position_candidates)
                .HasForeignKey(d => d.position_id)
                .HasConstraintName("fk_position_candidates_position_id_positions_id");
        });

        modelBuilder.Entity<position_contact>(entity =>
        {
            entity.HasKey(e => e.id).HasName("position_contacts_pkey");

            entity.HasOne(d => d.company).WithMany(p => p.position_contacts)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_position_contacts_company_id_companies_id");

            entity.HasOne(d => d.contact).WithMany(p => p.position_contacts)
                .HasForeignKey(d => d.contact_id)
                .HasConstraintName("fk_position_contacts_contact_id_contacts_id");

            entity.HasOne(d => d.position).WithMany(p => p.position_contacts)
                .HasForeignKey(d => d.position_id)
                .HasConstraintName("fk_position_contacts_position_id_positions_id");
        });

        modelBuilder.Entity<position_interviewer>(entity =>
        {
            entity.HasKey(e => e.id).HasName("position_interviewers_pkey");

            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.company).WithMany(p => p.position_interviewers)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_position_interviewers_company_id_companies_id");

            entity.HasOne(d => d.position).WithMany(p => p.position_interviewers)
                .HasForeignKey(d => d.position_id)
                .HasConstraintName("fk_position_interviewers_position_id_positions_id");

            entity.HasOne(d => d.user).WithMany(p => p.position_interviewers)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("fk_position_interviewers_user_id_users_id");
        });

        modelBuilder.Entity<position_type>(entity =>
        {
            entity.HasKey(e => e.id).HasName("position_types_pkey");

            entity.Property(e => e.date_updated)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.type_name).HasMaxLength(150);

            entity.HasOne(d => d.company).WithMany(p => p.position_types)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_position_types_company_id_companies_id");
        });

        modelBuilder.Entity<registeration_key>(entity =>
        {
            entity.HasKey(e => e.id).HasName("registeration_key_pkey");

            entity.ToTable("registeration_key");

            entity.Property(e => e.id).HasMaxLength(100);
            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(250);
        });

        modelBuilder.Entity<search>(entity =>
        {
            entity.HasKey(e => e.id).HasName("searches_pkey");

            entity.HasIndex(e => e.search_date, "ix_searches_search_date");

            entity.Property(e => e.advanced_val).HasMaxLength(150);
            entity.Property(e => e.search_date)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.val).HasMaxLength(150);

            entity.HasOne(d => d.company).WithMany(p => p.searches)
                .HasForeignKey(d => d.company_id)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("searches_company_id_companies_id");
        });

        modelBuilder.Entity<sent_email>(entity =>
        {
            entity.HasKey(e => e.id).HasName("sent_emails_pkey");

            entity.Property(e => e.body).HasMaxLength(1000);
            entity.Property(e => e.date_sent)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.subject).HasMaxLength(500);
            entity.Property(e => e.to).HasMaxLength(500);

            entity.HasOne(d => d.company).WithMany(p => p.sent_emails)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_sent_emails_company_id_companies_id");
        });

        modelBuilder.Entity<user>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_pkey");

            entity.Property(e => e.date_created)
                .HasDefaultValueSql("now()")
                .HasColumnType("timestamp without time zone");
            entity.Property(e => e.email).HasMaxLength(250);
            entity.Property(e => e.first_name).HasMaxLength(20);
            entity.Property(e => e.first_name_en).HasMaxLength(20);
            entity.Property(e => e.last_name).HasMaxLength(20);
            entity.Property(e => e.last_name_en).HasMaxLength(20);
            entity.Property(e => e.log_info).HasMaxLength(1500);
            entity.Property(e => e.mail_password).HasMaxLength(50);
            entity.Property(e => e.mail_username).HasMaxLength(50);
            entity.Property(e => e.passwaord).HasMaxLength(120);
            entity.Property(e => e.phone).HasMaxLength(20);
            entity.Property(e => e.signature).HasMaxLength(10000);

            entity.HasOne(d => d.company).WithMany(p => p.users)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_users_company_id_companies_id");
        });

        modelBuilder.Entity<users_refresh_token>(entity =>
        {
            entity.HasKey(e => e.id).HasName("users_refresh_tokens_pkey");

            entity.Property(e => e.token).HasMaxLength(200);
            entity.Property(e => e.token_expire).HasColumnType("timestamp without time zone");

            entity.HasOne(d => d.company).WithMany(p => p.users_refresh_tokens)
                .HasForeignKey(d => d.company_id)
                .HasConstraintName("fk_refresh_tokens_company_id_companies_id");

            entity.HasOne(d => d.user).WithMany(p => p.users_refresh_tokens)
                .HasForeignKey(d => d.user_id)
                .HasConstraintName("fk_refresh_tokens_user_id_users_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
