using Database.models;
using DataModelsLibrary.Models;

namespace DataModelsLibrary.Queries
{
    public interface ICvsPositionsQueries
    {
        public void AddImportedCv(ImportCvModel importCv);
        public candidate AddNewCandidate(int companyId, string email, string phone);
        public candidate? GetCandidateByEmail(string email);
        public List<CompanyTextToIndexModel> GetCompanyCvsToIndex(int companyId);
        public int GetUniqueCvId();
    }
}