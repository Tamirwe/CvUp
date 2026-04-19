using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;

namespace CandsPositionsLibrary
{
    public class CandsCvsService : ICandsCvsService
    {
        private ICandsCvsQueries _candsCvsQueries;

        public CandsCvsService( ICandsCvsQueries candsCvsQueries)
        {
            _candsCvsQueries = candsCvsQueries;
        }

        public async Task<List<AiCvModel>>  GetCandsCvsTextParams(int companyId = 154, int candidateId = 0)
        {
            List<AiCvModel> cvPropsToIndexList = await _candsCvsQueries.GetDistinctCandsCvs(companyId, candidateId);
            return cvPropsToIndexList;
        }
    }
}




