using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;

namespace DataModelsLibrary.Queries
{
    public class LuceneQueries : ILuceneQueries
    {

        public async Task<List<CandLastCvModel>> AllCandidatesLastCv()
        {
            using var dbContext = new cvupdbContext();
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            string sql = $@"
                WITH valid_cvs AS (
                         SELECT candidate_id, MAX(cv_id) AS max_cv_id
                         FROM public.cvs_txt
                         WHERE cv_txt IS NOT NULL AND TRIM(cv_txt) <> ''
                         GROUP BY candidate_id
                     )
                     SELECT ctx.candidate_id AS candidateId,
                            ctx.cv_id        AS cvId,
                            cnd.first_name   AS firstName,
                            cnd.last_name    AS lastName,
                            cnd.review  AS reviewText,
                            cnd.email,
                            cnd.phone,
                            ctx.cv_txt       AS cvTxt
                     FROM public.cvs_txt ctx
                     INNER JOIN candidates cnd ON cnd.id = ctx.candidate_id
                     INNER JOIN valid_cvs v ON v.candidate_id = ctx.candidate_id AND v.max_cv_id = ctx.cv_id
                     WHERE cnd.is_black_list = false
                     ORDER BY ctx.candidate_id DESC";

            return await dbContext.candLastCv.FromSqlRaw(sql).ToListAsync();
        }

        public async Task<CandLastCvModel?> CandidateLastCv(int candidateId)
        {
            using var dbContext = new cvupdbContext();
            dbContext.ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;

            string sql = $@"
                   SELECT ctx.candidate_id AS candidateId,
                   ctx.cv_id        AS cvId,
                   cnd.first_name   AS firstName,
                   cnd.last_name    AS lastName,
                   cnd.review       AS reviewText,
                   cnd.email,
                   cnd.phone,
                   ctx.cv_txt       AS cvTxt
                        FROM public.cvs_txt ctx
                        INNER JOIN candidates cnd ON cnd.id = ctx.candidate_id
                        WHERE ctx.candidate_id = {candidateId}
                        AND cnd.is_black_list = false
                        AND ctx.cv_txt IS NOT NULL AND TRIM(ctx.cv_txt) <> ''
                        AND ctx.cv_id = (
                            SELECT MAX(cv_id)
                            FROM public.cvs_txt
                            WHERE candidate_id = {candidateId}
                            AND cv_txt IS NOT NULL AND TRIM(cv_txt) <> ''
                        )";

            return (await dbContext.candLastCv.FromSqlRaw(sql).ToListAsync()).FirstOrDefault();
        }

    }
}
