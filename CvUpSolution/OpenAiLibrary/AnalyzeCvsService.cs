using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAiLibrary;

namespace OpenAiLibrary
{

    public class AnalyzeCvsService : IAnalyzeCvsService
    {
        private OpenAIClient client;
        private ChatClient chatClient;

        private ICandsCvsQueries _candsCvsQueries;
        private List<IsraeliCities> citiesRegion;


        public AnalyzeCvsService(ICandsCvsQueries candsCvsQueries)
        {
            _candsCvsQueries = candsCvsQueries;
            client = new OpenAIClient(apiKey);
            chatClient = client.GetChatClient("gpt-4o-mini");
        }

        public async Task AiAnalyzeAndStoreAllCandidatesLastCv(int companyId = 154)
        {
            var AnalyzedCvsList = new List<AnalyzedCvModel>();

            try
            {
                await LoadJsonRegionCitiesAsync();
                List<CandCvTxtModel> allCandidatesLastCvList = await GetCandsLastCvText(companyId);


                foreach (var candCv in allCandidatesLastCvList)
                {
                    var analyzedCvResult = await AnalyzeCv(candCv);
                    SaveAnalyzedCv(analyzedCvResult, candCv.candidateId);
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private void SaveAnalyzedCv(AnalyzedCvModel analyzedCvResult, int candidateId)
        {
            ai_analyze_cv analyzeCv = new ai_analyze_cv();
            analyzeCv.candidate_id = candidateId;
            analyzeCv.name = analyzedCvResult.Name;
            analyzeCv.email = analyzedCvResult.Email;
            analyzeCv.phone = analyzedCvResult.Phone;
            analyzeCv.city = analyzedCvResult.Location;
            analyzeCv.region = analyzedCvResult.Region;
            analyzeCv.summary = analyzedCvResult.Summary;
            analyzeCv.current_title = analyzedCvResult.CurrentTitle;
            analyzeCv.languages = string.Join(", ", analyzedCvResult.Languages);
            analyzeCv.seniority = analyzedCvResult.Seniority;
            analyzeCv.skills = string.Join(", ", analyzedCvResult.Skills);
            analyzeCv.years_experience = analyzedCvResult.YearsExperience;

            _candsCvsQueries.AddCandidateAnalyzeAI(analyzeCv);
        }

        private async Task LoadJsonRegionCitiesAsync()
        {
            string jsonString = await File.ReadAllTextAsync("israeliCities.json");
            citiesRegion = JsonConvert.DeserializeObject<List<IsraeliCities>>(jsonString)!;
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
                if (locationRecord != null) AnalyzedCv.Region = locationRecord.region;
            }

            return AnalyzedCv;
        }




    }
}

public class IsraeliCities
{
    [JsonProperty("city")] public required string city { get; set; }
    [JsonProperty("region")] public required string region { get; set; }
    [JsonProperty("area")] public required string area { get; set; }
}

public class AnalyzedCvModel
{
    [JsonProperty("name")] public string? Name { get; set; }
    [JsonProperty("email")] public string? Email { get; set; }
    [JsonProperty("phone")] public string? Phone { get; set; }
    [JsonProperty("location")] public string? Location { get; set; }
    [JsonProperty("skills")] public List<string> Skills { get; set; } = [];
    [JsonProperty("seniority")] public string Seniority { get; set; } = "Unknown";
    [JsonProperty("years_experience")] public int YearsExperience { get; set; }
    [JsonProperty("current_title")] public string? CurrentTitle { get; set; }
    [JsonProperty("languages")] public List<string> Languages { get; set; } = [];
    [JsonProperty("summary")] public string Summary { get; set; } = "";

    [Newtonsoft.Json.JsonIgnore] public CvLanguage CvLanguage { get; set; }
    [Newtonsoft.Json.JsonIgnore] public string? Region { get; set; }
}