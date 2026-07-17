using DataModelsLibrary.Models;
using System.Threading.Tasks;

namespace DataModelsLibrary.Queries
{
    public interface IBlackCandQueries
    {
        Task AddBlackCand(blackCandModel blackCand);
        Task RemoveBlackCand(int candidateId);
    }
}
