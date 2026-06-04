using DataModelsLibrary.Models;
using FuzzySharp;
using GeneralLibrary;
using GeneralLibrary.IsraelCities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenAI;
using OpenAI.Chat;

namespace CvAnalyzeEmbedOpenAiLibrary
{
    public class AnalyzeCvOpenAi : IAnalyzeCvOpenAi
    {

        private OpenAIClient client;
        private ChatClient chatClient;

        private List<IsraeliCitiesModel> _citiesRegionList;
        private string promptForCvAnalyze = "";

        public AnalyzeCvOpenAi(IConfiguration configuration, List<IsraeliCitiesModel> citiesRegionList)
        {
            var apiKey = configuration["API_KEY"];

            client = new OpenAIClient(apiKey);
            chatClient = client.GetChatClient("gpt-4o-mini");
            promptForCvAnalyze = File.ReadAllText("cv_prompt.txt");
            _citiesRegionList = citiesRegionList;
        }

        public async Task<AnalyzedCvModel?> AiAnalyzeCv(int candId, int cvId, string? cvText)
        {
            string? json = null;

            try
            {
                if (string.IsNullOrWhiteSpace(cvText))
                {
                    return null;
                }

                var cvLanguage = CleanString.DetectStringLanguage(cvText);

                string textCv = CleanString.RemovePunctuationAndNormelizeHebrew(cvText, cvLanguage);

                var messages = new List<ChatMessage>
                    {
                        new SystemChatMessage(promptForCvAnalyze),
                        new UserChatMessage(textCv)
                    };

                var chatOptions = new ChatCompletionOptions
                {
                    Temperature = 0.2f,
                    ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
                };

                var completion = await chatClient.CompleteChatAsync(messages, chatOptions);

                json = completion.Value.Content[0].Text;

                AnalyzedCvModel AnalyzedCv = ParseResult(json);

                (List<string>, List<string>, List<string>) JobsTitles = splitWorkExperience(AnalyzedCv.WorkExperience);
                (List<string>, List<string>) professionWordsHeEn = splitHeEnList(AnalyzedCv.ProfessionWords);
                (List<string>, List<string>) professionSkillsHeEn = splitHeEnList(AnalyzedCv.ProfessionSkills);

                AnalyzedCv.Companies = JobsTitles.Item1;
                AnalyzedCv.JobsTitlesHe = JobsTitles.Item2;
                AnalyzedCv.JobsTitlesEn = JobsTitles.Item3;
                AnalyzedCv.professionWordsHe = professionWordsHeEn.Item1;
                AnalyzedCv.professionWordsEn = professionWordsHeEn.Item2;
                AnalyzedCv.professionSkillsHe = professionSkillsHeEn.Item1;
                AnalyzedCv.professionSkillsEn = professionSkillsHeEn.Item2;

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
                    EstimateAge = myParseInt(obj.Value<string>("estimate_age")),
                    Email = obj.Value<string>("email"),
                    Phone = obj.Value<string>("phone"),
                    CityHe = obj.Value<string>("city_he"),
                    Languages = obj.Value<string>("languages"),
                    WorkExperience = obj["work_experience"]?.ToObject<List<string>>() ?? [],
                    ProfessionWords = obj["profession_words"]?.ToObject<List<string>>() ?? [],
                    ProfessionSkills = obj["profession_skills"]?.ToObject<List<string>>() ?? [],
                    Seniority = obj.Value<string>("seniority"),
                    Education = obj["education_he"]?.ToObject<List<string>>() ?? [],
                    Skills = obj["skills"]?.ToObject<List<string>>() ?? [],
                    MilitaryService = obj.Value<string>("military_service_he"),
                    SummaryEn = obj.Value<string>("summary_en") ?? "",
                    SummaryHe = obj.Value<string>("summary_he") ?? "",
                    YearsExperience = myParseInt(obj.Value<string>("years_experience")),
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
        private (List<string>, List<string>, List<string>) splitWorkExperience(List<string>? workExperience)
        {
            if (workExperience == null || workExperience.Count == 0)
            {
                return ([], [], []);
            }

            List<string> companies = [];
            List<string> jobsTitlesEn = [];
            List<string> jobsTitlesHe = [];

            foreach (var item in workExperience)
            {
                var splitArr = item.Split("::");

                companies.Add(splitArr.Length > 0 ? splitArr[0].Trim() : "");
                jobsTitlesEn.Add(splitArr.Length > 1 ? splitArr[1].Trim() : "");
                jobsTitlesHe.Add(splitArr.Length > 2 ? splitArr[2].Trim() : "");
            }

            return (companies, jobsTitlesEn, jobsTitlesHe);
        }

        private (List<string>, List<string>) splitHeEnList(List<string> professionWordsEn)
        {
            if (professionWordsEn.Count == 0)
            {
                return ([], []);
            }

            List<string> heList = [];
            List<string> enList = [];

            foreach (var item in professionWordsEn)
            {
                var heEnSplit = splitHeEnString(item);
                heList.Add(heEnSplit.Item1);
                enList.Add(heEnSplit.Item2);
            }

            return (heList, enList);
        }

        private (string, string) splitHeEnString(string? strEnHe)
        {
            if (string.IsNullOrWhiteSpace(strEnHe))
            {
                return ("", "");
            }

            var splitArr = strEnHe.Split("::");
            return (splitArr[0].Trim(), splitArr[1].Trim());
        }

        private (string?, string?) FindAreaRegion(string? location)
        {
            string? area = null, region = null;

            if (!string.IsNullOrWhiteSpace(location) && _citiesRegionList != null)
            {
                location = location.Replace("קיבוץ", "").Replace("קריית", "קרית").Trim();
                location = location.Contains("תל אביב") ? "תל אביב - יפו" : location;


                IsraeliCitiesModel? locationRecord = _citiesRegionList.FirstOrDefault(x => x.city == location);

                if (locationRecord == null)
                {
                    locationRecord = _citiesRegionList.Where(item => Fuzz.Ratio(location, item.city) > 70).ToList().FirstOrDefault();
                }

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
