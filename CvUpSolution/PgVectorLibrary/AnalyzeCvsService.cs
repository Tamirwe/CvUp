using CvAnalyzeEmbedOpenAiLibrary;
using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using QueueLibrary;
using GL = GeneralLibrary;

namespace PgVectorLibrary
{
    public class AnalyzeCvsService: IAnalyzeCvsService
    {

        private readonly IAiQueries _aiQueries;
        private readonly IAnalyzeCvOpenAi _analyzeCvOpenAi;
        private readonly IDbQueueService _queueService;
        private readonly int _companyId;

        public AnalyzeCvsService(IAiQueries aiQueries, IAnalyzeCvOpenAi analyzeCvOpenAi, IDbQueueService queueService, int companyId = 154)
        {
            _aiQueries = aiQueries;
            _analyzeCvOpenAi = analyzeCvOpenAi;
            _queueService = queueService;
            _companyId = companyId;
        }

        public async Task AnalyzeCandidatesLastCv()
        {
            List<CandCvTxtModel> candsLastCvList = await _aiQueries.GetCandsLastCvText(_companyId, 0);

            int counter = 0, total = candsLastCvList.Count;

            foreach (var candCv in candsLastCvList)
            {
                try
                {
                    AnalyzedCvModel? analyzedCv = await _analyzeCvOpenAi.AiAnalyzeCv(candCv.candidateId, candCv.cvId, candCv.cvTxt);
                    await SaveAnalyzedCv(analyzedCv);

                    Console.WriteLine($"Analyzed candidate {candCv.candidateId}  ({++counter}/{total})");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  problem: {ex.Message}");
                    //throw ex;
                }
            }
        }

        public async Task<bool> AnalyzeCvFromQueue()
        {
            var job = await _queueService.DequeueAsync("analyze new cv", "AnalyzeCvsService");

            if (job == null) return false;

            try
            {
                int candidateId = int.Parse(job.payload);
                List<CandCvTxtModel> cvList = await _aiQueries.GetCandsLastCvText(_companyId, candidateId);

                if (cvList.Count == 0)
                {
                    await _queueService.CompleteAsync(job.id);
                    return true;
                }

                var candCv = cvList[0];
                AnalyzedCvModel? analyzedCv = await _analyzeCvOpenAi.AiAnalyzeCv(candCv.candidateId, candCv.cvId, candCv.cvTxt);
                await SaveAnalyzedCv(analyzedCv);

                await _queueService.CompleteAsync(job.id);
                Console.WriteLine($"Queue analyzed candidate {candidateId}");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Queue analyze failed: {ex.Message}");
                await _queueService.FailAsync(job.id);
                return true;
            }
        }

        private async Task SaveAnalyzedCv(AnalyzedCvModel? analyzedCv)
        {
            if (analyzedCv == null)
            {
                return;
            }

            analyzed_cv analyzeCv = new analyzed_cv();
            analyzeCv.candidate_id = analyzedCv.CandidateId;
            analyzeCv.cv_id = analyzedCv.CvId;
            analyzeCv.name = GL.UtilsStr.limitLen(analyzedCv.Name, 255);
            analyzeCv.estimate_age = (short?)analyzedCv.EstimateAge;
            analyzeCv.email = GL.UtilsStr.limitLen(analyzedCv.Email, 255);
            analyzeCv.phone = GL.UtilsStr.limitLen(analyzedCv.Phone, 50);
            analyzeCv.city_he = GL.UtilsStr.limitLen(analyzedCv.CityHe, 50);
            analyzeCv.region = GL.UtilsStr.limitLen(analyzedCv.Region, 20);
            analyzeCv.area = GL.UtilsStr.limitLen(analyzedCv.Area, 20);
            analyzeCv.languages = analyzedCv.Languages;
            analyzeCv.skills = analyzedCv.Skills;
            analyzeCv.work_experience = analyzedCv.WorkExperience;
            analyzeCv.profession_words = analyzedCv.ProfessionWords;
            analyzeCv.education = analyzedCv.Education;
            analyzeCv.seniority_he = GL.UtilsStr.limitLen(analyzedCv.SeniorityHe, 50);
            analyzeCv.seniority_en = GL.UtilsStr.limitLen(analyzedCv.SeniorityEn, 50);
            analyzeCv.military_service_he = GL.UtilsStr.limitLen(analyzedCv.MilitaryServiceHe, 255);
            analyzeCv.summary_en = GL.UtilsStr.limitLen(analyzedCv.SummaryEn, 1000);
            analyzeCv.summary_he = GL.UtilsStr.limitLen(analyzedCv.SummaryHe, 1000);
            analyzeCv.years_experience = (short?)analyzedCv.YearsExperience;

            await _aiQueries.AddCandidateAnalyzeCv(analyzeCv);
        }

      
    }
}
