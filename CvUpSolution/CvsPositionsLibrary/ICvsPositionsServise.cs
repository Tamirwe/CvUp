using DataModelsLibrary.Models;

namespace CvsPositionsLibrary
{
    public interface ICvsPositionsServise
    {
        public void GetAddCandidateId(ImportCvModel item);
        public int GetUniqueCvId();
    }
}