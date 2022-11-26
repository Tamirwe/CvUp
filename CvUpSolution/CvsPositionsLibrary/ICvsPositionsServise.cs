using DataModelsLibrary.Models;

namespace CvsPositionsLibrary
{
    public interface ICvsPositionsServise
    {
        public void AddImportedCv(string companyId, string cvId, int candidateId, int cvAsciiSum, string emailId, string subject, string from);
        public void GetAddCandidateId(ImportCvModel item);
        public int GetUniqueCvId();
        public void BuildCompanyLuceneIndex(int companyId);

    }
}