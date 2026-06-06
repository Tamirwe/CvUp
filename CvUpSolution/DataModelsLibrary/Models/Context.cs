using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;

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

        }
    }
}
