using CvAnalyzeEmbedOpenAiLibrary;
using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using GL = GeneralLibrary;

namespace PgVectorLibrary
{
    public class AnalyzeCvsService: IAnalyzeCvsService
    {

        private readonly ICandsCvsQueries _candsCvsQueries;
        private readonly IAnalyzeCvOpenAi _analyzeCvOpenAi;
        private readonly int _companyId;

        public AnalyzeCvsService(ICandsCvsQueries candsCvsQueries, IAnalyzeCvOpenAi analyzeCvOpenAi,  int companyId = 154)
        {

            _candsCvsQueries = candsCvsQueries;
            _analyzeCvOpenAi = analyzeCvOpenAi;
            _companyId = companyId;
        }

        public async Task AnalyzeCandidatesLastCv()
        {
            List<CandCvTxtModel> candsLastCvList = await _candsCvsQueries.GetCandsLastCvText(_companyId);

            foreach (var candCv in candsLastCvList)
            {
                try
                {
                    AnalyzedCvModel? analyzedCv = await _analyzeCvOpenAi.AiAnalyzeCv(candCv.candidateId, candCv.id, candCv.cvTxt);
                    await SaveAnalyzedCv(analyzedCv);

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

            ai_analyze_cv analyzeCv = new ai_analyze_cv();
            analyzeCv.candidate_id = analyzedCv.CandidateId;
            analyzeCv.cv_id = analyzedCv.CvId;
            analyzeCv.name = GL.UtilsStr.limitLen(analyzedCv.Name, 101);
            analyzeCv.estimate_age = analyzedCv.EstimateAge;
            analyzeCv.email = GL.UtilsStr.limitLen(analyzedCv.Email, 150);
            analyzeCv.phone = GL.UtilsStr.limitLen(analyzedCv.Phone, 20);
            analyzeCv.city = GL.UtilsStr.limitLen(analyzedCv.CityHe, 50);
            analyzeCv.region = GL.UtilsStr.limitLen(analyzedCv.Region, 20);
            analyzeCv.area = GL.UtilsStr.limitLen(analyzedCv.Area, 20);
            analyzeCv.languages = GL.UtilsStr.limitLen(analyzedCv.Languages, 150);
            analyzeCv.jobs_titles_en = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.JobsTitlesEn), 500);
            analyzeCv.jobs_titles_he = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.JobsTitlesHe), 500);
            analyzeCv.profession_words_en = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.professionWordsEn), 500);
            analyzeCv.profession_words_he = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.professionWordsHe), 500);
            analyzeCv.profession_skills_en = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.professionSkillsEn), 500);
            analyzeCv.profession_skills_he = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.professionSkillsHe), 500);
            analyzeCv.seniority = GL.UtilsStr.limitLen(analyzedCv.Seniority, 50);
            analyzeCv.education = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.Education), 500);
            analyzeCv.companies = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.Companies), 500);
            analyzeCv.skills = GL.UtilsStr.limitLen(string.Join(", ", analyzedCv.Skills), 600);
            analyzeCv.military_service = GL.UtilsStr.limitLen(analyzedCv.MilitaryService, 250);
            analyzeCv.summary_en = GL.UtilsStr.limitLen(analyzedCv.SummaryEn, 1000);
            analyzeCv.summary_he = GL.UtilsStr.limitLen(analyzedCv.SummaryHe, 1000);
            analyzeCv.years_experience = analyzedCv.YearsExperience;

            await _candsCvsQueries.AddCandidateAnalyzeCv(analyzeCv);
        }

      
    }
}
