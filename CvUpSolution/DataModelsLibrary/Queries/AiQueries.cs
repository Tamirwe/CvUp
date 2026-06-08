using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using Pgvector;

namespace DataModelsLibrary.Queries
{
    public class AiQueries : IAiQueries
    {
        public async Task UpdateCvEmbedding(int candidateId, float[]? titlesEmbedding, float[]? skillsEmbedding, float[]? summaryEmbedding, float[]? companiesEmbedding)
        {
            using var dbContext = new cvupdbContext();

            var setClauses = new List<string>();
            if (titlesEmbedding != null)    setClauses.Add($"titles_embedding = '{new Vector(titlesEmbedding)}'");
            if (skillsEmbedding != null)    setClauses.Add($"skills_embedding = '{new Vector(skillsEmbedding)}'");
            if (summaryEmbedding != null)   setClauses.Add($"summary_embedding = '{new Vector(summaryEmbedding)}'");
            if (companiesEmbedding != null) setClauses.Add($"companies_embedding = '{new Vector(companiesEmbedding)}'");

            setClauses.Add("is_embedded = true");

            await dbContext.Database.ExecuteSqlRawAsync(
                $"UPDATE analyzed_cvs SET {string.Join(", ", setClauses)} WHERE candidate_id = {candidateId}");
        }

        public async Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId = 154, int candidateId = 0)
        {
            using var dbContext = new cvupdbContext();
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            string candidateFilter = candidateId > 0 ? $"AND ctx.candidate_id = {candidateId}" : "";

            string sql = sql = $@"
                    WITH valid_cvs AS (
                        SELECT candidate_id, MAX(cv_id) AS max_cv_id
                        FROM public.cvs_txt
                        WHERE cv_txt IS NOT NULL AND TRIM(cv_txt) <> ''
                        GROUP BY candidate_id
                    )
                    SELECT ctx.id, ctx.candidate_id AS candidateId, ctx.cv_id AS cvId, ctx.cv_txt AS cvTxt
                    FROM public.cvs_txt ctx
                    INNER JOIN candidates cnd ON cnd.id = ctx.candidate_id
                    INNER JOIN valid_cvs v ON v.candidate_id = ctx.candidate_id AND v.max_cv_id = ctx.cv_id
                    WHERE cnd.is_cv_analyzed = false
                    {candidateFilter}
                    ORDER BY ctx.candidate_id DESC
                    LIMIT 500";


            return await dbContext.candCvTxtModel.FromSqlRaw(sql).ToListAsync();
        }

        public async Task AddCandidateAnalyzeCv(analyzed_cv analyzeCv)
        {
            await DeleteCandidateAnalyzeCv(analyzeCv.candidate_id);

            using var dbContext = new cvupdbContext();
            dbContext.analyzed_cvs.Add(analyzeCv);
            await dbContext.SaveChangesAsync();

            await UpdateCandIsAnalyzed(analyzeCv.candidate_id, true);
        }

        public async Task DeleteCandidateAnalyzeCv(int candidateId)
        {
            using var dbContext = new cvupdbContext();

            analyzed_cv? record = await dbContext.analyzed_cvs
                .FirstOrDefaultAsync(x => x.candidate_id == candidateId);

            if (record != null)
            {
                dbContext.analyzed_cvs.Remove(record);
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

            var query = from ai in dbContext.analyzed_cvs
                        where ai.is_embedded == false
                        select new AnalyzedCvsForEmbeedingModel
                        {
                            CandidateId     = ai.candidate_id,
                            CvId            = ai.cv_id,
                            Name            = ai.name,
                            Email           = ai.email,
                            EstimateAge     = ai.estimate_age,
                            Phone           = ai.phone,
                            Location        = ai.city_he,
                            Region          = ai.region,
                            Area            = ai.area,
                            Languages       = ai.languages,
                            Skills          = ai.skills,
                            SeniorityHe     = ai.seniority_he,
                            SeniorityEn     = ai.seniority_en,
                            Education       = ai.education,
                            MilitaryService = ai.military_service_he,
                            WorkExperience  = ai.work_experience,
                            ProfessionWords = ai.profession_words,
                            SummaryEn       = ai.summary_en,
                            SummaryHe       = ai.summary_he,
                            YearsExperience = ai.years_experience,
                        };

            return await query.ToListAsync();
        }

       

        public async Task<List<AiCandidateSearchModel>> SearchCvsByEmbedding(float[] queryVector, int limit = 20)
        {
            using var dbContext = new cvupdbContext();
            var v = new Vector(queryVector).ToString();
            var sql = $@"
                SELECT
                    a.candidate_id     AS candidateId,
                    a.cv_id            AS cvId,
                    a.name,
                    a.city_he          AS city,
                    a.work_experience  AS workExperience,
                    a.estimate_age     AS age,
                    a.education,
                    a.summary_he       AS summary,
                    (
                        CASE WHEN a.titles_embedding    IS NOT NULL THEN (a.titles_embedding    <=> '{v}') * 0.70 ELSE 0 END +
                        CASE WHEN a.skills_embedding    IS NOT NULL THEN (a.skills_embedding    <=> '{v}') * 0.12 ELSE 0 END +
                        CASE WHEN a.summary_embedding   IS NOT NULL THEN (a.summary_embedding   <=> '{v}') * 0.13 ELSE 0 END +
                        CASE WHEN a.companies_embedding IS NOT NULL THEN (a.companies_embedding <=> '{v}') * 0.5 ELSE 0 END
                    ) / NULLIF(
                        CASE WHEN a.titles_embedding    IS NOT NULL THEN 0.70 ELSE 0 END +
                        CASE WHEN a.skills_embedding    IS NOT NULL THEN 0.12 ELSE 0 END +
                        CASE WHEN a.summary_embedding   IS NOT NULL THEN 0.13 ELSE 0 END +
                        CASE WHEN a.companies_embedding IS NOT NULL THEN 0.5 ELSE 0 END
                    , 0) AS distance
                FROM analyzed_cvs a
                WHERE a.is_embedded = true
                ORDER BY distance
                LIMIT {limit}";

            var results = await dbContext.candidateSearchResults.FromSqlRaw(sql).ToListAsync();

            foreach (var r in results)
            {
                if (string.IsNullOrWhiteSpace(r.workExperience)) continue;
                var arr = JArray.Parse(r.workExperience);
                r.companies    = arr.Select(x => x.Value<string>("company")  ?? "").Where(s => s != "").ToList();
                r.jobsTitlesHe = arr.Select(x => x.Value<string>("title_he") ?? "").Where(s => s != "").ToList();
            }

            return results;
        }

    }
}
