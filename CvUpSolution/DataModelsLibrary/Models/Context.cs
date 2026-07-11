using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace Database.models
{
    public partial class cvupdbContext : DbContext
    {
        public virtual DbSet<IdNameModel> idNameModelDB { get; set; } = null!;
        public virtual DbSet<CandCvTxtModel> candCvTxtModel { get; set; } = null!;
        public virtual DbSet<AiCandidateSearchModel> candidateSearchResults { get; set; } = null!;
        public virtual DbSet<CvsToIndexModel> cvsToIndex { get; set; } = null!;
        public virtual DbSet<CandLastCvModel> candLastCv { get; set; } = null!;

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AiCandidateSearchModel>().HasNoKey();

        }
    }
}
