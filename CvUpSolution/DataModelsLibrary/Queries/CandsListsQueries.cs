using Database.models;
using DataModelsLibrary.Models;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DataModelsLibrary.Queries
{
    public class CandsListsQueries : ICandsListsQueries
    {
        public CandsListsQueries()
        {
        }

        public async Task<List<CandModel?>> GetCandsList(int companyId, List<int>? candsIds)
        {
            using (var dbContext = new cvupdbContext())
            {
                var dbQuery = from cand in dbContext.candidates
                              join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                              join acvJoin in dbContext.analyzed_cvs on cand.id equals acvJoin.candidate_id into acvGroup
                              from acv in acvGroup.DefaultIfEmpty()
                              where cand.company_id == companyId
                              orderby cand.last_cv_sent descending
                              select new
                              {
                                  CvId = cvs.id,
                                  cand.review,
                                  cand.review_date,
                                  cand.customers_reviews,
                                  cvs.key_id,
                                  CandidateId = cand.id,
                                  cand.email,
                                  EmailSubject = cvs.subject,
                                  cand.first_name,
                                  cand.last_name,
                                  cand.phone,
                                  cand.city,
                                  cand.has_duplicates_cvs,
                                  cand.last_cv_sent,
                                  cand.folders_ids,
                                  cand.pos_ids,
                                  cand.pos_stages,
                                  cvs.is_seen,
                                  cand.is_black_list,
                                  EstimateAge = acv != null ? acv.estimate_age : null,
                                  SeniorityHe = acv != null ? acv.seniority_he : null,
                                  WorkExperience = acv != null ? acv.work_experience : null,
                                  SummaryHe = acv != null ? acv.summary_he : null,
                                  Education = acv != null ? acv.education : null
                              };

                if (candsIds != null)
                {
                    dbQuery = dbQuery.Where(cand => candsIds.Contains(cand.CandidateId));
                }

                var rawResults = await dbQuery.Take(300).ToListAsync();

                List<CandModel> finalCandidates = rawResults.Select(cand => new CandModel
                {
                    cvId = cand.CvId,
                    review = cand.review,
                    reviewDate = cand.review_date,
                    allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                    keyId = cand.key_id,
                    candidateId = cand.CandidateId,
                    email = cand.email,
                    emailSubject = cand.EmailSubject,
                    firstName = cand.first_name,
                    lastName = cand.last_name,
                    phone = cand.phone,
                    city = cand.city,
                    hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                    cvSent = Convert.ToDateTime(cand.last_cv_sent),
                    candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                    candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                    posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                    isSeen = Convert.ToBoolean(cand.is_seen),
                    isBlackList = Convert.ToBoolean(cand.is_black_list),
                    EstimateAgeAI = (int?)cand.EstimateAge,
                    SeniorityHeAI = cand.SeniorityHe,
                    WorkExperienceAI = cand.WorkExperience == null ? null : JsonConvert.DeserializeObject<WorkExperienceItemModel[]>(cand.WorkExperience),
                    SummaryAI = cand.SummaryHe,
                    EducationAI = cand.Education == null ? null : JsonConvert.DeserializeObject<EducationItemModel[]>(cand.Education)
                }).ToList();

                return finalCandidates;
            }
        }

        public async Task<List<CandModel?>> GetPosCandsList(int companyId, int positionId)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = (from cand in dbContext.candidates
                             join pcv in dbContext.position_candidates on cand.id equals pcv.candidate_id
                             join cvs in dbContext.cvs on pcv.cv_id equals cvs.id
                             where pcv.company_id == companyId
                                    && pcv.position_id == positionId
                             orderby pcv.date_updated descending
                             select new CandModel
                             {
                                 cvId = pcv.cv_id,
                                 posCvId = pcv.cv_id,
                                 customerReview = pcv.customer_review,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = cvs.date_created,
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen),
                                 candPosHistory = new CandPosHistoryModel
                                 {
                                     accepted = pcv.accepted,
                                     callEmailToCandidate = pcv.call_email_to_candidate,
                                     customerInterview = pcv.customer_interview,
                                     emailToContact = pcv.email_to_contact,
                                     rejected = pcv.rejected,
                                     rejectEmailToCandidate = pcv.reject_email_to_candidate,
                                     removeCandidacy = pcv.remove_candidacy,
                                 }
                             });

                return await query.ToListAsync();
            }
        }

        public async Task<List<CandModel?>> GetPosTypeCandsList(int companyId, int positionTypeId)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = (from cand in dbContext.candidates
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where cand.company_id == companyId && cvs.position_type_id == positionTypeId
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen)
                             }).Take(300);

                return await query.ToListAsync();
            }
        }

        public async Task<List<CandModel?>> GetFolderCandsList(int companyId, int folderId)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = (from cand in dbContext.candidates
                             join fc in dbContext.folders_cands on cand.id equals fc.candidate_id
                             join cvs in dbContext.cvs on cand.last_cv_id equals cvs.id
                             where fc.company_id == companyId
                                    && fc.folder_id == folderId
                             orderby cand.last_cv_sent descending
                             select new CandModel
                             {
                                 cvId = cvs.id,
                                 review = cand.review,
                                 reviewDate = cand.review_date,
                                 allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                 keyId = cvs.key_id,
                                 candidateId = cand.id,
                                 email = cand.email,
                                 emailSubject = cvs.subject,
                                 firstName = cand.first_name,
                                 lastName = cand.last_name,
                                 phone = cand.phone,
                                 city = cand.city,
                                 hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                 cvSent = cvs.date_created,
                                 candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                 candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                 posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                 isSeen = Convert.ToBoolean(cvs.is_seen)
                             });

                return await query.ToListAsync();
            }
        }

        public async Task<CandModel?> GetPositionCandidate(int companyId, int candId, int positionId)
        {
            using (var dbContext = new cvupdbContext())
            {
                var query = (from cand in dbContext.candidates
                             join pcv in dbContext.position_candidates on cand.id equals pcv.candidate_id
                             join cvs in dbContext.cvs on pcv.cv_id equals cvs.id
                             where cand.company_id == companyId && cand.id == candId && pcv.position_id == positionId
                             select new CandModel
                             {
                                cvId = cvs.id,
                                review = cand.review,
                                reviewDate = cand.review_date,
                                allCustomersReviews = cand.customers_reviews == null ? null : JsonConvert.DeserializeObject<CandCustomersReviewsModel[]>(cand.customers_reviews),
                                keyId = cvs.key_id,
                                candidateId = cand.id,
                                email = cand.email,
                                emailSubject = cvs.subject,
                                firstName = cand.first_name,
                                lastName = cand.last_name,
                                phone = cand.phone,
                                city = cand.city,
                                hasDuplicates = Convert.ToBoolean(cand.has_duplicates_cvs),
                                cvSent = Convert.ToDateTime(cand.last_cv_sent),
                                candFoldersIds = cand.folders_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.folders_ids),
                                candPosIds = cand.pos_ids == null ? new int[] { } : JsonConvert.DeserializeObject<int[]>(cand.pos_ids),
                                posStages = cand.pos_stages == null ? null : JsonConvert.DeserializeObject<CandPosStageModel[]>(cand.pos_stages),
                                isSeen = Convert.ToBoolean(cvs.is_seen),
                                candPosHistory = new CandPosHistoryModel
                                {
                                    accepted = pcv.accepted,
                                    callEmailToCandidate = pcv.call_email_to_candidate,
                                    customerInterview = pcv.customer_interview,
                                    emailToContact = pcv.email_to_contact,
                                    rejected = pcv.rejected,
                                    rejectEmailToCandidate = pcv.reject_email_to_candidate,
                                    removeCandidacy = pcv.remove_candidacy,
                                }
                             }).Take(300);

                var result = await query.FirstOrDefaultAsync();
                return result;
            }
        }
    }
}
