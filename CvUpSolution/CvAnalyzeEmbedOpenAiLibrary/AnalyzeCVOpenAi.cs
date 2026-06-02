using CvAnalyzeEmbedOpenAiLibrary.Models;
using FuzzySharp;
using GeneralLibrary;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using OpenAI;
using OpenAI.Chat;

namespace CvAnalyzeEmbedOpenAiLibrary
{
    public class AnalyzeCvOpenAi : IAnalyzeCvOpenAi
    {

        private OpenAIClient client;
        private ChatClient chatClient;

        private List<IsraeliCitiesModel>? _citiesRegion;
        private string promptForCvAnalyze = "";

        public AnalyzeCvOpenAi( IConfiguration configuration)
        {
            var apiKey = configuration["API_KEY"];

            client = new OpenAIClient(apiKey);
            chatClient = client.GetChatClient("gpt-4o-mini");
            promptForCvAnalyze = File.ReadAllText("cv_prompt.txt");
            string israeliCitiesString =  File.ReadAllText("israeliCities.json");
            _citiesRegion = JsonConvert.DeserializeObject<List<IsraeliCitiesModel>>(israeliCitiesString)!;
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

                    AnalyzedCvModel AnalyzedCv = ParseAiResult.ParseResult(json);

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




        private static string? limitLen(string? original, int maxLength)
        {
            if (original != null)
            {
                return original.Substring(0, Math.Min(original.Length, maxLength));
            }
            return null;
        }

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

                companies.Add(splitArr[0].Trim());
                jobsTitlesEn.Add(splitArr[1].Trim());
                jobsTitlesHe.Add(splitArr[2].Trim());

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

            if (!string.IsNullOrWhiteSpace(location) && _citiesRegion != null)
            {
                location = location.Replace("קיבוץ", "").Replace("קריית", "קרית").Trim();
                location = location.Contains("תל אביב") ? "תל אביב - יפו" : location;

                
                IsraeliCitiesModel? locationRecord = _citiesRegion.FirstOrDefault(x => x.city == location);

                if (locationRecord == null)
                {
                    locationRecord = _citiesRegion.Where(item => Fuzz.Ratio(location, item.city) > 70).ToList().FirstOrDefault();
                }

                if (locationRecord != null)
                {
                    area = locationRecord.region.Trim();
                    region = locationRecord.area.Trim();
                }
            }

            return (area, region);
        }

    }
}
