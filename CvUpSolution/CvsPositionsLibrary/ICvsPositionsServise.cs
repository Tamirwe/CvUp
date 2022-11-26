using DataModelsLibrary.Models;

namespace CvsPositionsLibrary
{
    public interface ICvsPositionsServise
    {
        public void AddImportedCv(ImportCvModel importCv);
        public void GetAddCandidateId(ImportCvModel item);
        public int GetUniqueCvId();
        public void IndexCompanyCvs(int companyId);

    }
}