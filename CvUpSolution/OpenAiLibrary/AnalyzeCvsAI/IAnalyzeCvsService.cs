using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary.AnalyzeCvsAI
{
    public interface IAnalyzeCvsService
    {
        Task AiAnalyzeAndStoreAllCandidatesLastCv(string apiKey, int companyId = 154);
    }
}
