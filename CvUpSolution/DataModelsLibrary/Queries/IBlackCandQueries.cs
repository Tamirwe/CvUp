using DataModelsLibrary.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public interface IBlackCandQueries
    {
        Task<List<blackCandModel>> GetBlackCandidatesList();
        Task UpdateBlackCandidateEmailCount(blackCandModel blackCand);
        Task AddBlackCand(blackCandModel blackCand);
        Task RemoveBlackCand(int candidateId);
    }
}
