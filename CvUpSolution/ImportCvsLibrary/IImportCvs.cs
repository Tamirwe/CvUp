namespace ImportCvsLibrary
{
    public interface IImportCvs
    {
        Task ImportFromGmail();
        void BackupDataBase();
    }
}