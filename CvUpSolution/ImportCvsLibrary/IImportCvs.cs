using DataModelsLibrary.Models;

namespace ImportCvsLibrary
{
    public interface IImportCvs
    {
        Task ImportFromGmail();
    }
}