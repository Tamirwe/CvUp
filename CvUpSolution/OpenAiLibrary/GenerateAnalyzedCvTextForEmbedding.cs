using DataModelsLibrary.Models;
using OpenAiLibrary.Embedding;
using Newtonsoft.Json.Linq;

namespace OpenAiLibrary
{
    public record CvEmbeddings(float[]? Titles, float[]? Skills, float[]? Summary, float[]? Companies);

    public class GenerateAnalyzedCvTextForEmbedding : IGenerateAnalyzedCvTextForEmbedding
    {
        private readonly IOpenAiEmbedding _embeddingOpenAi;

        public GenerateAnalyzedCvTextForEmbedding(IOpenAiEmbedding embeddingOpenAi)
        {
            _embeddingOpenAi = embeddingOpenAi;
        }

        public async Task<CvEmbeddings> EmbedCv(AnalyzedCvsForEmbeedingModel analyzeCv)
        {
            ParseWorkExperience(analyzeCv);
            ParseProfessionWords(analyzeCv);

            var titlesText    = Join(analyzeCv.JobsTitlesHe, analyzeCv.JobsTitlesEn, analyzeCv.ProfessionWordsHe, analyzeCv.ProfessionWordsEn);
            var skillsText    = Join(analyzeCv.Skills);
            var summaryText   = Join(analyzeCv.SummaryHe, analyzeCv.SummaryEn);
            var companiesText = Join(analyzeCv.Companies);

            return new CvEmbeddings(
                Titles:    await _embeddingOpenAi.EmbedText(titlesText),
                Skills:    await _embeddingOpenAi.EmbedText(skillsText),
                Summary:   await _embeddingOpenAi.EmbedText(summaryText),
                Companies: await _embeddingOpenAi.EmbedText(companiesText)
            );
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
    }
}
