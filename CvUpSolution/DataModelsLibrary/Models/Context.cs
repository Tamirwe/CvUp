using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Pgvector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Database.models
{
    public partial class cvupdbContext : DbContext
    {
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql("Host=localhost;Database=cvupdb;Username=postgres;Password=!Shalot5");
            }
        }

        public virtual DbSet<IdNameModel> idNameModelDB { get; set; } = null!;
        public virtual DbSet<CandCvTxtModel> candCvTxtModel { get; set; } = null!;
        public virtual DbSet<CandidateSearchResultModel> candidateSearchResults { get; set; } = null!;

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CandidateSearchResultModel>().HasNoKey();

            var embeddingProp = modelBuilder.Entity<ai_analyze_cv>()
                .Property(e => e.embedding)
                .HasColumnType("vector(1536)")
                .HasConversion(
                    v => v == null ? null! : v.ToString(),
                    v => v == null ? null! : new Vector(v));

            embeddingProp.Metadata.SetBeforeSaveBehavior(PropertySaveBehavior.Ignore);
            embeddingProp.Metadata.SetAfterSaveBehavior(PropertySaveBehavior.Ignore);
        }
    }
}
