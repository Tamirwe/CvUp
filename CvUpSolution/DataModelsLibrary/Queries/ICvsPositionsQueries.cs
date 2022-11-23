using Database.models;

namespace DataModelsLibrary.Queries
{
    public interface ICvsPositionsQueries
    {
        public void AddImportedCv(string companyId, string cvId, int candidateId, string cvTxt, string emailId, string subject, string from);
        public candidate AddNewCandidate(string email, string phone);
        public candidate? GetCandidateByEmail(string email);
        public int GetUniqueCvId();
    }
}