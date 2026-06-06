using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Pgvector;

namespace DataModelsLibrary.Queries
{
    public class AiQueries : IAiQueries
    {
        public async Task UpdateCvEmbedding(int candidateId, float[] embedding)
        {
            using var dbContext = new cvupdbContext();
            var vectorLiteral = new Vector(embedding).ToString();
            await dbContext.Database.ExecuteSqlRawAsync(
                $"UPDATE ai_analyze_cvs SET embedding = '{vectorLiteral}', is_embedded = true WHERE candidate_id = {candidateId}");
        }

        public async Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId = 154, int candidateId = 0)
        {
            using var dbContext = new cvupdbContext();
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            string sql = @"SELECT  ctx.cv_id id, ctx.candidate_id candidateId, ctx.cv_txt cvTxt
                            FROM cvs_txt ctx
                            WHERE ctx.cv_id IN ( SELECT cv_id FROM (SELECT cands.last_cv_id cv_id
	                                FROM candidates cands
	                                WHERE cands.company_id = " + companyId + @" AND cands.is_cv_analyzed = false
	                                ORDER BY cands.id DESC
	                                LIMIT  20) AS tbl)";

            return await dbContext.candCvTxtModel.FromSqlRaw(sql).ToListAsync();
        }

        public async Task AddCandidateAnalyzeCv(ai_analyze_cv analyzeCv)
        {
            await DeleteCandidateAnalyzeCv(analyzeCv.candidate_id);

            using var dbContext = new cvupdbContext();
            dbContext.ai_analyze_cvs.Add(analyzeCv);
            await dbContext.SaveChangesAsync();

            await UpdateCandIsAnalyzed(analyzeCv.candidate_id, true);
        }

        public async Task DeleteCandidateAnalyzeCv(int candidateId)
        {
            using var dbContext = new cvupdbContext();

            ai_analyze_cv? record = await dbContext.ai_analyze_cvs
                .FirstOrDefaultAsync(x => x.candidate_id == candidateId);

            if (record != null)
            {
                dbContext.ai_analyze_cvs.Remove(record);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateCandIsAnalyzed(int candidateId, bool isAnalyzed)
        {
            using var dbContext = new cvupdbContext();

            candidate? cand = dbContext.candidates.FirstOrDefault(x => x.id == candidateId);

            if (cand != null)
            {
                cand.is_cv_analyzed = isAnalyzed;
                dbContext.candidates.Update(cand);
                await dbContext.SaveChangesAsync();
            }
        }

        public async Task<List<AnalyzedCvsForEmbeedingModel>> GetAnalyzedCvsForEmbeeding()
        {
            using var dbContext = new cvupdbContext();

            var query = from ai in dbContext.ai_analyze_cvs
                        where ai.is_embedded == false
                        select new AnalyzedCvsForEmbeedingModel
                        {
                            CandidateId = ai.candidate_id,
                            CvId = ai.cv_id,
                            Name = ai.name,
                            Email = ai.email,
                            EstimateAge = ai.estimate_age,
                            Phone = ai.phone,
                            Location = ai.city,
                            Region = ai.region,
                            Area = ai.area,
                            Languages = ai.languages,
                            JobsTitlesEn = StringToList(ai.jobs_titles_en),
                            JobsTitlesHe = StringToList(ai.jobs_titles_he),
                            ProfessionWordsEn = StringToList(ai.profession_words_en),
                            ProfessionWordsHe = StringToList(ai.profession_words_he),
                            ProfessionSkillsEn = StringToList(ai.profession_skills_en),
                            ProfessionSkillsHe = StringToList(ai.profession_skills_he),
                            Seniority = ai.seniority,
                            Education = ai.education,
                            Companies = StringToList(ai.companies),
                            Skills = StringToList(ai.skills),
                            MilitaryService = ai.military_service,
                            SummaryEn = ai.summary_en ?? "",
                            SummaryHe = ai.summary_he ?? "",
                            YearsExperience = ai.years_experience,
                        };

            return await query.Take(300).ToListAsync();
        }

        public async Task<List<CandidateSearchResultModel>> SearchCvsByEmbedding(float[] queryVector,  int limit = 20)
        {
            using var dbContext = new cvupdbContext();
            var vectorLiteral = new Vector(queryVector).ToString();
            var sql = $@"SELECT a.candidate_id AS candidateId, a.cv_id AS cvId, a.name, a.city, a.jobs_titles_he AS jobsTitles,
                         a.profession_words_he AS professionWords, a.estimate_age AS age, a.education, a.companies, a.summary_he AS summary,
                         a.embedding <=> '{vectorLiteral}' AS distance 
                         FROM ai_analyze_cvs a
                         JOIN candidates c ON c.id = a.candidate_id
                         WHERE a.is_embedded = true
                         ORDER BY distance
                         LIMIT {limit}";
            return await dbContext.candidateSearchResults.FromSqlRaw(sql).ToListAsync();
        }

        private static List<string>? StringToList(string? str)
        {
            if (str == null) return null;
            return str.Split(',').ToList();
        }
    }
}
