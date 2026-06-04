using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using FuzzySharp;
using GeneralLibrary;
using Microsoft.Extensions.Configuration;
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
        private List<IsraeliCitiesModel>? citiesRegion;
        private string promptForCvAnalyze = "";
        private readonly int _companyId;

        public AnalyzeCvsService(ICandsCvsQueries candsCvsQueries, IConfiguration configuration,  int companyId = 154)
        {
            var apiKey =  configuration["API_KEY"];

            _candsCvsQueries = candsCvsQueries;
            client = new OpenAIClient(apiKey);
            chatClient = client.GetChatClient("gpt-4o-mini");
            promptForCvAnalyze = File.ReadAllText("AnalyzeCvsAI\\cv_prompt.txt");
            _companyId = companyId;
        }

        public async Task AiAnalyzeNewCvs()
        {
            await LoadJsonRegionCitiesAsync();

            List<CandCvTxtModel> candidatesCvToAnalyzeList = await _candsCvsQueries.GetCandsLastCvText(_companyId);
            await AiAnalyzeAndStoreCandidatesLastCv(candidatesCvToAnalyzeList);
        }

        public async Task AiAnalyzeOldCvs()
        {
            await LoadJsonRegionCitiesAsync();

            List<CandCvTxtModel> candidatesCvToAnalyzeList = await _candsCvsQueries.GetCandsLastCvText(_companyId);
            await AiAnalyzeAndStoreCandidatesLastCv(candidatesCvToAnalyzeList);
        }

        private async Task AiAnalyzeAndStoreCandidatesLastCv(List<CandCvTxtModel> candidatesCvToAnalyzeList)
        {
            string? json;

            foreach (var candCv in candidatesCvToAnalyzeList)
            {
                json = null;

                try
                {
                    if (string.IsNullOrWhiteSpace(candCv.cvTxt))
                    {
                        continue;
                    }

                    var cvLanguage = CleanString.DetectStringLanguage(candCv.cvTxt);

                    string textCv = CleanString.RemovePunctuationAndNormelizeHebrew(candCv.cvTxt , cvLanguage);

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

                    AnalyzedCvModelOLD AnalyzedCv = ParseAiResult.ParseResult(json);

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
                    AnalyzedCv.CandidateId = candCv.candidateId;
                    AnalyzedCv.CvId = candCv.id;

                    (string?, string?) areaRegion = FindAreaRegion(AnalyzedCv.CityHe);

                    if (areaRegion.Item1 != null)
                    {
                        AnalyzedCv.Region = areaRegion.Item1;
                        AnalyzedCv.Area = areaRegion.Item2;
                    }

                    await SaveAnalyzedCv(AnalyzedCv);

                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  problem: {ex.Message}");
                    Console.WriteLine($" json {json[..Math.Min(200, json.Length)]}");
                    //throw ex;
                }
            }
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

            if (!string.IsNullOrWhiteSpace(location))
            {
                location = location.Replace("קיבוץ", "").Replace("קריית", "קרית").Trim();
                location = location.Contains("תל אביב") ? "תל אביב - יפו" : location;


                IsraeliCitiesModel? locationRecord = citiesRegion.FirstOrDefault(x => x.city == location);

                if (locationRecord == null)
                {
                    locationRecord = citiesRegion.Where(item => Fuzz.Ratio(location, item.city) > 70).ToList().FirstOrDefault();
                }

                if (locationRecord != null)
                {
                    area = locationRecord.region.Trim();
                    region = locationRecord.area.Trim();
                }
            }

            return (area, region);
        }

        private static string? limitLen(string? original, int maxLength)
        {
            if (original != null)
            {
                return original.Substring(0, Math.Min(original.Length, maxLength));
            }
            return null;
        }

        private async Task SaveAnalyzedCv(AnalyzedCvModelOLD analyzedCvResult)
        {
            ai_analyze_cv analyzeCv = new ai_analyze_cv();
            analyzeCv.candidate_id = analyzedCvResult.CandidateId;
            analyzeCv.cv_id = analyzedCvResult.CvId;
            analyzeCv.name = limitLen(analyzedCvResult.Name, 101);
            analyzeCv.estimate_age = analyzedCvResult.EstimateAge;
            analyzeCv.email = limitLen(analyzedCvResult.Email, 150);
            analyzeCv.phone = limitLen(analyzedCvResult.Phone, 20);
            analyzeCv.city = limitLen(analyzedCvResult.CityHe, 50);
            analyzeCv.region = limitLen(analyzedCvResult.Region, 20);
            analyzeCv.area = limitLen(analyzedCvResult.Area, 20);
            analyzeCv.languages = limitLen(analyzedCvResult.Languages, 150);
            analyzeCv.jobs_titles_en = limitLen(string.Join(", ", analyzedCvResult.JobsTitlesEn), 500);
            analyzeCv.jobs_titles_he = limitLen(string.Join(", ", analyzedCvResult.JobsTitlesHe), 500);
            analyzeCv.profession_words_en = limitLen(string.Join(", ", analyzedCvResult.professionWordsEn), 500);
            analyzeCv.profession_words_he = limitLen(string.Join(", ", analyzedCvResult.professionWordsHe), 500);
            analyzeCv.profession_skills_en = limitLen(string.Join(", ", analyzedCvResult.professionSkillsEn), 500);
            analyzeCv.profession_skills_he = limitLen(string.Join(", ", analyzedCvResult.professionSkillsHe), 500);
            analyzeCv.seniority = limitLen(analyzedCvResult.Seniority, 50);
            analyzeCv.education = limitLen(string.Join(", ", analyzedCvResult.Education), 500);
            analyzeCv.companies = limitLen(string.Join(", ", analyzedCvResult.Companies), 500);
            analyzeCv.skills = limitLen(string.Join(", ", analyzedCvResult.Skills), 600);
            analyzeCv.military_service = limitLen(analyzedCvResult.MilitaryService, 250);
            analyzeCv.summary_en = limitLen(analyzedCvResult.SummaryEn, 1000);
            analyzeCv.summary_he = limitLen(analyzedCvResult.SummaryHe, 1000);
            analyzeCv.years_experience = analyzedCvResult.YearsExperience;

            await _candsCvsQueries.AddCandidateAnalyzeCv(analyzeCv);
        }

        private async Task LoadJsonRegionCitiesAsync()
        {
            string jsonString = await File.ReadAllTextAsync("AnalyzeCvsAI\\israeliCities.json");
            citiesRegion = JsonConvert.DeserializeObject<List<IsraeliCitiesModel>>(jsonString)!;
        }
             
    }
}











//public async Task AiAnalyzeAndStoreAllCandidatesLastCv(int companyId = 154)
//{
//    await LoadJsonRegionCitiesAsync();
//    List<CandCvTxtModel> allCandidatesLastCvList = await _candsCvsQueries.GetCandsLastCvText(companyId);

//    foreach (var candCv in allCandidatesLastCvList)
//    {
//        try
//        {
//            var analyzedCvResult = await AnalyzeCv(candCv, (int)candCv.candidateId!);
//            SaveAnalyzedCv(analyzedCvResult);
//        }
//        catch (Exception ex)
//        {
//            //throw ex;

//        }
//    }
//}



//private async Task<AnalyzedCvModel> AnalyzeCv(CandCvTxtModel candCv, int candidateId)
//{
//    string cv = NormalizeTextForAI.NormalizeCvText(candCv.cvTxt ?? "");
//    var cvLanguage = LanguageDetector.Detect(cv);

//    var messages = new List<ChatMessage>
//    {
//        new SystemChatMessage(" Extract structured CV data as JSON."),
//        new UserChatMessage($@"
//You are a senior HR analyst and career coach.
//Extract structured data from the CV below and return ONLY a valid JSON in HEBREW.
//No explanation, no markdown, no code fences — raw JSON only.
//JSON schema (all fields required, use empty string or empty array if unknown):

//- name
//- email
//- phone
//- location
//- skills
//- years_experience
//- current_title
//- languages // text description
//- summary // naturally mention experience level (e.g. בכיר, זוטר, מנוסה) if it can be inferred from the CV

//If the CV is in English – translate everything to Hebrew.

//CV:
//{cv}
//")
//    };

//    var response = await chatClient.CompleteChatAsync(messages);
//    var json = response.Value.Content[0].Text;

//    var AnalyzedCv = ParseAiResult.ParseResult(json);
//    //AnalyzedCv.CvLanguage = cvLanguage;
//    AnalyzedCv.CandidateId = candidateId;

//    //if (!string.IsNullOrWhiteSpace(AnalyzedCv.Location))
//    //{
//    //    var locationRecord = citiesRegion.FirstOrDefault(x => x.city == AnalyzedCv.Location);
//    //    if (locationRecord != null)
//    //    {
//    //        AnalyzedCv.Region = locationRecord.region;
//    //        AnalyzedCv.Area = locationRecord.area;
//    //    }
//    //}

//    return AnalyzedCv;
//}
