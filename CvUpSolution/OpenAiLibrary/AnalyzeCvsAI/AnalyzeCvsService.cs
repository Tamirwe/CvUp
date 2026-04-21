using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using MySqlX.XDevAPI.Common;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAiLibrary.Models;

namespace OpenAiLibrary.AnalyzeCvsAI
{

    public class AnalyzeCvsService : IAnalyzeCvsService
    {
        private OpenAIClient client;
        private ChatClient chatClient;

        private ICandsCvsQueries _candsCvsQueries;
        private List<IsraeliCitiesModel> citiesRegion;



        public AnalyzeCvsService(ICandsCvsQueries candsCvsQueries)
        {
            _candsCvsQueries = candsCvsQueries;
            client = new OpenAIClient(apiKey);
            chatClient = client.GetChatClient("gpt-4o-mini");
        }

        public async Task AiAnalyzeAndStoreAllCandidatesLastCv(int companyId = 154)
        {
            var AnalyzedCvsList = new List<AnalyzedCvModel>();


            await LoadJsonRegionCitiesAsync();
            List<CandCvTxtModel> allCandidatesLastCvList = await GetCandsLastCvText(companyId);

            foreach (var candCv in allCandidatesLastCvList)
            {
                try
                {
                    var analyzedCvResult = await AnalyzeCv(candCv);
                    SaveAnalyzedCv(analyzedCvResult, (int)candCv.candidateId!);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private void SaveAnalyzedCv(AnalyzedCvModel analyzedCvResult, int candidateId)
        {
            int yearsExperience = 0;


            if (int.TryParse(analyzedCvResult.YearsExperience, out int result))
            {
                yearsExperience = result;
            }

            ai_analyze_cv analyzeCv = new ai_analyze_cv();
            analyzeCv.candidate_id = candidateId;
            analyzeCv.name = limitLen(analyzedCvResult.Name, 101);
            analyzeCv.email = limitLen(analyzedCvResult.Email, 150);
            analyzeCv.phone = limitLen(analyzedCvResult.Phone, 20);
            analyzeCv.city = limitLen(analyzedCvResult.Location, 50);
            analyzeCv.region = limitLen(analyzedCvResult.Region, 20);
            analyzeCv.area = limitLen(analyzedCvResult.Area, 20);
            analyzeCv.summary = limitLen(analyzedCvResult.Summary, 1000);
            analyzeCv.current_title = limitLen(analyzedCvResult.CurrentTitle, 101);
            analyzeCv.languages = limitLen(string.Join(", ", analyzedCvResult.Languages), 150);
            analyzeCv.seniority = analyzedCvResult.Seniority;
            analyzeCv.skills = limitLen(string.Join(", ", analyzedCvResult.Skills), 1000);
            analyzeCv.years_experience = yearsExperience;

            _candsCvsQueries.AddCandidateAnalyzeCv(analyzeCv);
        }

        private async Task LoadJsonRegionCitiesAsync()
        {
            string jsonString = await File.ReadAllTextAsync("israeliCities.json");
            citiesRegion = JsonConvert.DeserializeObject<List<IsraeliCitiesModel>>(jsonString)!;
        }

        private async Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId)
        {
            List<CandCvTxtModel> allCandidatesLastCvList = await _candsCvsQueries.GetCandsLastCvText(companyId);
            return allCandidatesLastCvList;
        }

        private async Task<AnalyzedCvModel> AnalyzeCv(CandCvTxtModel candCv)
        {
            string cv = NormalizeTextForAI.NormalizeCvText(candCv.cvTxt ?? "");
            var cvLanguage = LanguageDetector.Detect(cv);

            var messages = new List<ChatMessage>
    {
        new SystemChatMessage(" Extract structured CV data as JSON."),
        new UserChatMessage($@"
You are a senior HR analyst and career coach.
Extract structured data from the CV below and return ONLY a valid JSON in HEBREW.
No explanation, no markdown, no code fences — raw JSON only.
JSON schema (all fields required, use empty string or empty array if unknown):

- name
- email
- phone
- location
- skills
- seniority // one of: Junior | Mid | Senior | Lead | Unknown
- years_experience
- current_title
- languages
- summary // 1 to 5 sentences candidate summary

If the CV is in English – translate everything to Hebrew.

CV:
{cv}
")
    };

            var response = await chatClient.CompleteChatAsync(messages);
            var json = response.Value.Content[0].Text;

            var AnalyzedCv = ParseAiResult.ParseResult(json);
            AnalyzedCv.CvLanguage = cvLanguage;

            if (!string.IsNullOrWhiteSpace(AnalyzedCv.Location))
            {
                var locationRecord = citiesRegion.FirstOrDefault(x => x.city == AnalyzedCv.Location);
                if (locationRecord != null)
                {
                    AnalyzedCv.Region = locationRecord.region;
                    AnalyzedCv.Area = locationRecord.area;
                }
            }

            return AnalyzedCv;
        }

        private static string? limitLen(string? original, int maxLength)
        {
            if (original != null)
            {
                return original.Substring(0, Math.Min(original.Length, maxLength));
            }
            return null;
        }



    }
}


