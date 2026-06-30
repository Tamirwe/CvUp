using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Newtonsoft.Json.Linq;
using OpenAiLibrary.AnalyzeCv;
using OpenAiLibrary.Embedding;
using GL = GeneralLibrary;

namespace PgVectorLibrary.AnalyzeCvs
{
    public class AnalyzeCvsService: IAnalyzeCvsService
    {

        private readonly IAiQueries _aiQueries;
        private readonly IOpenAiAnalyzeCvService _analyzeCvOpenAi;
        private readonly IOpenAiEmbeddingService _embeddingOpenAi;
        private readonly int _companyId;

        public AnalyzeCvsService(IAiQueries aiQueries, IOpenAiAnalyzeCvService analyzeCvOpenAi, IOpenAiEmbeddingService embeddingOpenAi, int companyId = 154)
        {
            _aiQueries = aiQueries;
            _analyzeCvOpenAi = analyzeCvOpenAi;
            _embeddingOpenAi = embeddingOpenAi;
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
                    await EmbedAnalyzeCvs(candCv.candidateId);

                    Console.WriteLine($"Analyzed candidate {candCv.candidateId}  ({++counter}/{total})");

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  problem: {ex.Message}");
                    //throw ex;
                }
            }
        }

        public async Task EmbedAnalyzeCvs(int candidateId = 0)
        {
            List<AnalyzedCvsForEmbeedingModel> analyzedCvsForEmbeedingList = await _aiQueries.GetAnalyzedCvsForEmbeeding(candidateId);

            foreach (var analyzeCv in analyzedCvsForEmbeedingList)
            {
                ParseWorkExperience(analyzeCv);
                ParseProfessionWords(analyzeCv);

                var titlesText    = Join(analyzeCv.JobsTitlesHe, analyzeCv.JobsTitlesEn, analyzeCv.ProfessionWordsHe, analyzeCv.ProfessionWordsEn);
                var skillsText    = Join(analyzeCv.Skills);
                var summaryText   = Join(analyzeCv.SummaryHe, analyzeCv.SummaryEn);
                var companiesText = Join(analyzeCv.Companies);

                var titles    = await _embeddingOpenAi.EmbedText(titlesText);
                var skills    = await _embeddingOpenAi.EmbedText(skillsText);
                var summary   = await _embeddingOpenAi.EmbedText(summaryText);
                var companies = await _embeddingOpenAi.EmbedText(companiesText);

                await _aiQueries.UpdateCvEmbedding(analyzeCv.CandidateId, titles, skills, summary, companies);
            }
        }

        private static void ParseWorkExperience(AnalyzedCvsForEmbeedingModel analyzeCv)
        {
            if (string.IsNullOrWhiteSpace(analyzeCv.WorkExperience)) return;

            var arr = JArray.Parse(analyzeCv.WorkExperience);
            foreach (var item in arr)
            {
                var company = item.Value<string>("company");
                var titleHe = item.Value<string>("title_he");
                var titleEn = item.Value<string>("title_en");

                if (!string.IsNullOrWhiteSpace(company)) analyzeCv.Companies.Add(company);
                if (!string.IsNullOrWhiteSpace(titleHe)) analyzeCv.JobsTitlesHe.Add(titleHe);
                if (!string.IsNullOrWhiteSpace(titleEn)) analyzeCv.JobsTitlesEn.Add(titleEn);
            }
        }

        private static void ParseProfessionWords(AnalyzedCvsForEmbeedingModel analyzeCv)
        {
            if (string.IsNullOrWhiteSpace(analyzeCv.ProfessionWords)) return;

            var arr = JArray.Parse(analyzeCv.ProfessionWords);
            foreach (var item in arr)
            {
                var he = item.Value<string>("hebrew");
                var en = item.Value<string>("english");

                if (!string.IsNullOrWhiteSpace(he)) analyzeCv.ProfessionWordsHe.Add(he);
                if (!string.IsNullOrWhiteSpace(en)) analyzeCv.ProfessionWordsEn.Add(en);
            }
        }

        private static string? Join(params IEnumerable<string?>?[] parts)
        {
            var text = string.Join(" ", parts
                .Where(p => p != null)
                .SelectMany(p => p!)
                .Where(s => !string.IsNullOrWhiteSpace(s)));
            return string.IsNullOrWhiteSpace(text) ? null : text;
        }

        private static string? Join(params string?[] parts)
        {
            var text = string.Join(" ", parts.Where(s => !string.IsNullOrWhiteSpace(s)));
            return string.IsNullOrWhiteSpace(text) ? null : text;
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
