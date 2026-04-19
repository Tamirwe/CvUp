using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenAiLibrary
{
    public interface IAnalyzeCvsService
    {
        Task<List<CandCvTxtModel>> GetCandsLastCvText(int companyId = 154, int candidateId = 0);
    }
}
