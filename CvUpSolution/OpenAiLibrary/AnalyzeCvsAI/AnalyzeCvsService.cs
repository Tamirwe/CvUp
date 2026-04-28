using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Google.Protobuf;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;
using OpenAiLibrary.EmbeddingAndStore;
using OpenAiLibrary.Models;
using Ubiety.Dns.Core;

namespace OpenAiLibrary.AnalyzeCvsAI
{

    public class AnalyzeCvsService : IAnalyzeCvsService
    {
        private OpenAIClient client;
        private ChatClient chatClient;

        private ICandsCvsQueries _candsCvsQueries;
        private List<IsraeliCitiesModel> citiesRegion;
        private string promptForCvAnalyze = "";

        public AnalyzeCvsService(ICandsCvsQueries candsCvsQueries, string apiKey)
        {
            _candsCvsQueries = candsCvsQueries;

            client = new OpenAIClient(apiKey);
            chatClient = client.GetChatClient("gpt-4o-mini");
            promptForCvAnalyze = File.ReadAllText("AnalyzeCvsAI\\cv_prompt.txt");


        }

        public async Task AiAnalyzeAndStoreAllCandidatesLastCvVer2(int companyId = 154)
        {
            await LoadJsonRegionCitiesAsync();

            List<CandCvTxtModel> allCandidatesLastCvList = await _candsCvsQueries.GetCandsLastCvText(companyId);


            foreach (var candCv in allCandidatesLastCvList)
            {
                try
                {
                    var cvLanguage = LanguageDetector.Detect(candCv.cvTxt ?? "");

                    var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage(promptForCvAnalyze),
                        new UserChatMessage(candCv.cvTxt)
                    };

                    var chatOptions = new ChatCompletionOptions
                    {
                        Temperature = 0.2f,
                        ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
                    };

                    var completion = await chatClient.CompleteChatAsync(messages, chatOptions );

                    var json = completion.Value.Content[0].Text;

                    AnalyzedCvModel AnalyzedCv = ParseAiResult.ParseResult(json);
                    AnalyzedCv.CvLanguage = cvLanguage;
                    AnalyzedCv.CandidateId = candCv.candidateId;
                    AnalyzedCv.CvId = candCv.id;
                    AnalyzedCv.KeyId = candCv.keyId;

                    if (!string.IsNullOrWhiteSpace(AnalyzedCv.Location))
                    {
                        var locationRecord = citiesRegion.FirstOrDefault(x => x.city == AnalyzedCv.Location);
                        if (locationRecord != null)
                        {
                            AnalyzedCv.Region = locationRecord.region;
                            AnalyzedCv.Area = locationRecord.area;
                        }
                    }

                    SaveAnalyzedCv(AnalyzedCv);

                }
                catch (Exception ex)
                {
                    //throw ex;

                }
            }
        }

        public async Task AiAnalyzeAndStoreAllCandidatesLastCv(int companyId = 154)
        {
            await LoadJsonRegionCitiesAsync();
            List<CandCvTxtModel> allCandidatesLastCvList = await GetCandsLastCvText(companyId);

            foreach (var candCv in allCandidatesLastCvList)
            {
                try
                {
                    var analyzedCvResult = await AnalyzeCv(candCv, (int)candCv.candidateId!);
                    SaveAnalyzedCv(analyzedCvResult);
                }
                catch (Exception ex)
                {
                    //throw ex;

                }
            }
        }

        private void SaveAnalyzedCv(AnalyzedCvModel analyzedCvResult)
        {
            ai_analyze_cv analyzeCv = new ai_analyze_cv();
            analyzeCv.candidate_id = analyzedCvResult.CandidateId;
            analyzeCv.name = limitLen(analyzedCvResult.Name, 101);
            analyzeCv.email = limitLen(analyzedCvResult.Email, 150);
            analyzeCv.phone = limitLen(analyzedCvResult.Phone, 20);
            analyzeCv.city = limitLen(analyzedCvResult.Location, 50);
            analyzeCv.region = limitLen(analyzedCvResult.Region, 20);
            analyzeCv.area = limitLen(analyzedCvResult.Area, 20);
            analyzeCv.current_title_en = limitLen(analyzedCvResult.CurrentTitleEn, 100);
            analyzeCv.current_title_he = limitLen(analyzedCvResult.CurrentTitleHe, 100);
            analyzeCv.companies = limitLen(string.Join(", ", analyzedCvResult.Companies), 1000);
            analyzeCv.skills = limitLen(string.Join(", ", analyzedCvResult.Skills), 1000);
            analyzeCv.summary_en = limitLen(analyzedCvResult.SummaryEn, 1000);
            analyzeCv.summary_he = limitLen(analyzedCvResult.SummaryHe, 1000);
            analyzeCv.languages = limitLen( analyzedCvResult.Languages, 150);
            analyzeCv.years_experience = analyzedCvResult.YearsExperience;

            _candsCvsQueries.AddCandidateAnalyzeCv(analyzeCv);
        }

        private async Task LoadJsonRegionCitiesAsync()
        {
            string jsonString = await File.ReadAllTextAsync("AnalyzeCvsAI\\israeliCities.json");
            citiesRegion = JsonConvert.DeserializeObject<List<IsraeliCitiesModel>>(jsonString)!;
        }

        private async Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId)
        {
            List<CandCvTxtModel> allCandidatesLastCvList = await _candsCvsQueries.GetCandsLastCvText(companyId);
            return allCandidatesLastCvList;
        }

        private async Task<AnalyzedCvModel> AnalyzeCv(CandCvTxtModel candCv, int candidateId)
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
- years_experience
- current_title
- languages // text description
- summary // naturally mention experience level (e.g. בכיר, זוטר, מנוסה) if it can be inferred from the CV

If the CV is in English – translate everything to Hebrew.

CV:
{cv}
")
    };

            var response = await chatClient.CompleteChatAsync(messages);
            var json = response.Value.Content[0].Text;

            var AnalyzedCv = ParseAiResult.ParseResult(json);
            AnalyzedCv.CvLanguage = cvLanguage;
            AnalyzedCv.CandidateId = candidateId;

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


