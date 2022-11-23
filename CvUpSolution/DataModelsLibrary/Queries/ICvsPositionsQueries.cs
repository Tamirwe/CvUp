using Database.models;

namespace DataModelsLibrary.Queries
{
    public interface ICvsPositionsQueries
    {
        public void AddCv();
        public candidate AddNewCandidate(string email, string phone);
        public candidate? GetCandidateByEmail(string email);
        public int GetUniqueCvId();
    }
}