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
        Task AiAnalyzeAndStoreAllCandidatesLastCv( int companyId = 154);
        Task AiAnalyzeAndStoreAllCandidatesLastCvVer2(int companyId = 154);
    }
}
