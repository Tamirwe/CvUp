namespace ImportCvsLibrary
{
    public interface IImportCvs
    {
        Task ImportFromGmail(List<string> blackCandidatesList);
    }
}