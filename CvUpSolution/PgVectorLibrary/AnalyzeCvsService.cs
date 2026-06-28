using AnalyzeEmbedOpenAiLibrary;
using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using GL = GeneralLibrary;

namespace PgVectorLibrary
{
    public class AnalyzeCvsService: IAnalyzeCvsService
    {

        private readonly IAiQueries _aiQueries;
        private readonly IAnalyzeCvOpenAi _analyzeCvOpenAi;
        private readonly IEmbedService _embedService;
        private readonly int _companyId;

        public AnalyzeCvsService(IAiQueries aiQueries, IAnalyzeCvOpenAi analyzeCvOpenAi, IEmbedService embedService, int companyId = 154)
        {
            _aiQueries = aiQueries;
            _analyzeCvOpenAi = analyzeCvOpenAi;
            _embedService = embedService;
            _companyId = companyId;
        }

        // this method analyzes candidates' CVs. If a candidateId is provided, it analyzes only that candidate's CV; otherwise, it analyzes all candidates' CVs that have not been analyzed yet.
        public async Task AnalyzeCandidates(int candidateId = 0)
        {
            List<CandLastCvModel> candsLastCvList = candidateId > 0
                ? new List<CandLastCvModel>(new[] { await _aiQueries.CandidateLastCvNotAnalysed(candidateId) }.Where(x => x != null)!)
                : await _aiQueries.AllCandidatesLastCvNotAnalysed();

            int counter = 0, total = candsLastCvList.Count;

            foreach (var candCv in candsLastCvList)
            {
                try
                {
                    AnalyzedCvModel? analyzedCv = await _analyzeCvOpenAi.AiAnalyzeCv(candCv.candidateId, candCv.cvId, candCv.cvTxt);
                    await SaveAnalyzedCv(analyzedCv);
                    await _embedService.EmbedAnalyzeCvs(candCv.candidateId);

                    Console.WriteLine($"Analyzed candidate {candCv.candidateId}  ({++counter}/{total})");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  problem: {ex.Message}");
                    //throw ex;
                }
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
