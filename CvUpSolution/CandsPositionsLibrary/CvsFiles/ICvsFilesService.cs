namespace CandsPositionsLibrary.CvsFiles
{
    public interface ICvsFilesService
    {
        Task ImportNewCvsExternalDisk(int companyId, string sourceFolder);
        public void RemoveUnRelatedCvsFiles();

    }
}
