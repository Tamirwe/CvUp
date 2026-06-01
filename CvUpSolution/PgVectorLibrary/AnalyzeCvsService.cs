using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;

namespace PgVectorLibrary
{
    public class AnalyzeCvsService: IAnalyzeCvsService
    {

        private ICandsCvsQueries _candsCvsQueries;
        private readonly int _companyId;

        public AnalyzeCvsService(ICandsCvsQueries candsCvsQueries,  int companyId = 154)
        {

            _candsCvsQueries = candsCvsQueries;
            _companyId = companyId;
        }

        //public async Task AnalyzeCandidatesLastCv()
        //{
        //    List<CandCvTxtModel> candsLastCvList = await _candsCvsQueries.GetCandsLastCvText(_companyId);

        //    string? json;

        //    foreach (var candCv in candsLastCvList)
        //    {
        //        json = null;

        //        try
        //        {
        //            if (string.IsNullOrWhiteSpace(candCv.cvTxt))
        //            {
        //                continue;
        //            }

        //            var cvLanguage = CleanString.DetectStringLanguage(candCv.cvTxt);

        //            string textCv = CleanString.RemovePunctuationAndNormelizeHebrew(candCv.cvTxt, cvLanguage);

        //            var messages = new List<ChatMessage>
        //            {
        //                new SystemChatMessage(promptForCvAnalyze),
        //                new UserChatMessage(textCv)
        //            };

        //            var chatOptions = new ChatCompletionOptions
        //            {
        //                Temperature = 0.2f,
        //                ResponseFormat = ChatResponseFormat.CreateJsonObjectFormat()
        //            };

        //            var completion = await chatClient.CompleteChatAsync(messages, chatOptions);

        //            json = completion.Value.Content[0].Text;

        //            AnalyzedCvModel AnalyzedCv = ParseAiResult.ParseResult(json);

        //            (List<string>, List<string>, List<string>) JobsTitles = splitWorkExperience(AnalyzedCv.WorkExperience);
        //            (List<string>, List<string>) professionWordsHeEn = splitHeEnList(AnalyzedCv.ProfessionWords);
        //            (List<string>, List<string>) professionSkillsHeEn = splitHeEnList(AnalyzedCv.ProfessionSkills);

        //            AnalyzedCv.Companies = JobsTitles.Item1;
        //            AnalyzedCv.JobsTitlesHe = JobsTitles.Item2;
        //            AnalyzedCv.JobsTitlesEn = JobsTitles.Item3;
        //            AnalyzedCv.professionWordsHe = professionWordsHeEn.Item1;
        //            AnalyzedCv.professionWordsEn = professionWordsHeEn.Item2;
        //            AnalyzedCv.professionSkillsHe = professionSkillsHeEn.Item1;
        //            AnalyzedCv.professionSkillsEn = professionSkillsHeEn.Item2;

        //            AnalyzedCv.CvLanguage = cvLanguage;
        //            AnalyzedCv.CandidateId = candCv.candidateId;
        //            AnalyzedCv.CvId = candCv.id;

        //            (string?, string?) areaRegion = FindAreaRegion(AnalyzedCv.CityHe);

        //            if (areaRegion.Item1 != null)
        //            {
        //                AnalyzedCv.Region = areaRegion.Item1;
        //                AnalyzedCv.Area = areaRegion.Item2;
        //            }

        //            await SaveAnalyzedCv(AnalyzedCv);

        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine($"  problem: {ex.Message}");
        //            Console.WriteLine($" json {json[..Math.Min(200, json.Length)]}");
        //            //throw ex;
        //        }
        //    }
        //}
    }
}
