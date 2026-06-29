using DataModelsLibrary.Models;
using FuzzySharp;
using GeneralLibrary;
using GeneralLibrary.IsraelCities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Chat;

namespace OpenAiLibrary.AnalyzeCv
{
    public class OpenAiAnalyzeCvService : IOpenAiAnalyzeCvService
    {

        private ChatClient? _chatClient;
        private readonly string? _apiKey;
        private readonly List<IsraeliCitiesModel> _citiesRegionList;
        private string? _prompt;

        public OpenAiAnalyzeCvService(IConfiguration configuration, List<IsraeliCitiesModel> citiesRegionList)
        {
            _apiKey = configuration["API_KEY"];
            _citiesRegionList = citiesRegionList;
        }

        private ChatClient ChatClient =>
            _chatClient ??= new OpenAIClient(_apiKey).GetChatClient("gpt-4o-mini");

        private string Prompt =>
            _prompt ??= File.ReadAllText(Path.Combine(AppContext.BaseDirectory, "cv_prompt.txt"));

        public async Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText)
        {
            string? json = null;

            try
            {
                if (string.IsNullOrWhiteSpace(cvText))
                {
                    return null;
                }

                var cvLanguage = StringMethods.DetectStringLanguage(cvText);

                string textCv = StringMethods.RemovePunctuationAndNormelizeHebrew(cvText, cvLanguage);

                var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage(Prompt),
                        new UserChatMessage(textCv)
                    };

                var chatOptions = new ChatCompletionOptions
                {
                    Temperature = 0.2f,
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
                };

                var completion = await ChatClient.CompleteChatAsync(messages, chatOptions);

                json = completion.Value.Content[0].Text;

                AnalyzedCvModel AnalyzedCv = ParseResult(json);

                (List<string>, List<string>, List<string>) JobsTitles = splitWorkExperience(AnalyzedCv.WorkExperience);
                (List<string>, List<string>) professionWordsHeEn = splitHeEnList(AnalyzedCv.ProfessionWords);

                AnalyzedCv.Companies = JobsTitles.Item1;
                AnalyzedCv.JobsTitlesHe = JobsTitles.Item2;
                AnalyzedCv.JobsTitlesEn = JobsTitles.Item3;
                AnalyzedCv.professionWordsHe = professionWordsHeEn.Item1;
                AnalyzedCv.professionWordsEn = professionWordsHeEn.Item2;

                AnalyzedCv.CvLanguage = cvLanguage;
                AnalyzedCv.CandidateId = candId;
                AnalyzedCv.CvId = cvId;

                (string?, string?) areaRegion = FindAreaRegion(AnalyzedCv.CityHe);

                if (areaRegion.Item1 != null)
                {
                    AnalyzedCv.Region = areaRegion.Item1;
                    AnalyzedCv.Area = areaRegion.Item2;
                }

                return AnalyzedCv;

            }
            catch (Exception ex)
            {
                Console.WriteLine($"  problem: {ex.Message}");
                Console.WriteLine($" json {json[..Math.Min(200, json.Length)]}");
                //throw ex;
            }

            return null;

        }

        #region Parse AI Result methods
        public static AnalyzedCvModel ParseResult(string json)
        {
            // Strip markdown fences if the model returns them anyway
            json = json
                .Replace("```json", "")
                .Replace("```", "")
                .Trim();

            // Extract the first { ... } block in case of extra text
            int start = json.IndexOf('{');
            int end = json.LastIndexOf('}');

            if (start >= 0 && end > start)
                json = json[start..(end + 1)];

            json = FixUnbalancedQuotes(json);

            try
            {

                var obj = JObject.Parse(json);

                return new AnalyzedCvModel
                {
                    Name = obj.Value<string>("name"),
                    EstimateAge = obj["estimate_age"]?.ToObject<int?>(),
                    Email = obj.Value<string>("email"),
                    Phone = obj.Value<string>("phone"),
                    CityHe = obj.Value<string>("city_he"),
                    Languages = obj["languages"]?.ToObject<List<string>>() ?? [],
                    WorkExperience = obj["work_experience"]?.ToString() ?? "[]",
                    ProfessionWords = obj["profession_words"]?.ToString() ?? "[]",
                    SeniorityHe = obj.Value<string>("seniority_he"),
                    SeniorityEn = obj.Value<string>("seniority_en"),
                    Education = obj["education"]?.ToString() ?? "[]",
                    Skills = obj["skills"]?.ToObject<List<string>>() ?? [],
                    MilitaryServiceHe = obj.Value<string>("military_service_he"),
                    MilitaryServiceEn = obj.Value<string>("military_service_en"),
                    SummaryEn = obj.Value<string>("summary_en") ?? "",
                    SummaryHe = obj.Value<string>("summary_he") ?? "",
                    YearsExperience = obj["years_experience"]?.ToObject<int?>(),
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  [warn] JSON parse failed: {ex.Message}");
                Console.WriteLine($"  [raw]  {json[..Math.Min(200, json.Length)]}");
                throw ex;

            }
        }

        private static int? myParseInt(string? val)
        {
            if (int.TryParse(val, out int result))
            {
                return result;
            }
            return null;
        }

        private static string FixUnbalancedQuotes(string json)
        {
            bool inString = false;
            var result = new System.Text.StringBuilder();

            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];

                if (c == '"' && (i == 0 || json[i - 1] != '\\'))
                {
                    inString = !inString;
                }

                result.Append(c);
            }

            // If still inside string → close it
            if (inString)
            {
                result.Append('"');
            }

            return result.ToString();
        }

        #endregion

        #region private methods for parsing and cleaning the ai result
        private (List<string>, List<string>, List<string>) splitWorkExperience(string? workExperienceJson)
        {
            if (string.IsNullOrWhiteSpace(workExperienceJson))
                return ([], [], []);

            var arr = JArray.Parse(workExperienceJson);

            List<string> companies = [];
            List<string> jobsTitlesHe = [];
            List<string> jobsTitlesEn = [];

            foreach (var item in arr)
            {
                companies.Add(item.Value<string>("company") ?? "");
                jobsTitlesHe.Add(item.Value<string>("title_he") ?? "");
                jobsTitlesEn.Add(item.Value<string>("title_en") ?? "");
            }

            return (companies, jobsTitlesHe, jobsTitlesEn);
        }

        private (List<string>, List<string>) splitHeEnList(string? json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return ([], []);

            var arr = JArray.Parse(json);

            List<string> heList = [];
            List<string> enList = [];

            foreach (var item in arr)
            {
                heList.Add(item.Value<string>("hebrew") ?? "");
                enList.Add(item.Value<string>("english") ?? "");
            }

            return (heList, enList);
        }

        private (string?, string?) FindAreaRegion(string? location)
        {
            string? area = null, region = null;

            if (!string.IsNullOrWhiteSpace(location) && _citiesRegionList != null)
            {
                location = location.Contains("תל אביב") ? "תל אביב - יפו" : location;

                var normalized = System.Text.RegularExpressions.Regex.Replace(
                    location.Replace("׳", "").Replace("'", "").Replace("'", "")
                            .Replace("״", "").Replace("\"", "")
                            .Replace("(", " ").Replace(")", " ").Replace("-", " ")
                            .Replace("קריית", "קרית"),
                    @"\s+", " ").Trim();

                IsraeliCitiesModel? locationRecord =
                    _citiesRegionList.FirstOrDefault(x => x.city_normalized == normalized);

                if (locationRecord == null)
                {
                    if (normalized.Contains("כפר") || normalized.Contains("קיבוץ"))
                    {
                        normalized = System.Text.RegularExpressions.Regex.Replace(
                        normalized.Replace("כפר", "").Replace("קיבוץ", ""),
                        @"\s+", " ").Trim();
                    }

                    locationRecord = _citiesRegionList.FirstOrDefault(x => x.city_normalized.Contains(normalized));
                }

                locationRecord ??= _citiesRegionList.FirstOrDefault(x => Fuzz.Ratio(normalized, x.city_normalized) > 70);

                if (locationRecord != null)
                {
                    area = locationRecord.region.Trim();
                    region = locationRecord.area.Trim();
                }
            }

            return (area, region);
        }

        #endregion

    }
}
