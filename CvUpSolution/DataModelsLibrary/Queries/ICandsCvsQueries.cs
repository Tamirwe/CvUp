using Database.models;
using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public interface ICandsCvsQueries
    {
        Task<List<AiCvModel>> GetDistinctCandsCvs(int companyId = 154, int candidateId = 0);
        Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId = 154, int candidateId = 0);
        Task AddCandidateAnalyzeAI(ai_analyze_cv analyzeCv);
    }
}
