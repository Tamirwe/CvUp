using Database.models;
using DataModelsLibrary.Queries;
using Microsoft.Extensions.Configuration;

namespace CandsPositionsLibrary.CvsFiles
{
    public class CvsFilesService : ICvsFilesService
    {
        private ICandsPositionsQueries _cvsPositionsQueries;
        string CvsRootFolder;

        public CvsFilesService(IConfiguration config, ICandsPositionsQueries cvsPositionsQueries)
        {
            _cvsPositionsQueries = cvsPositionsQueries;
            CvsRootFolder = $"{config["GlobalSettings:CvsFilesRootFolder"]}";
        }

        public async Task ImportNewCvsExternalDisk(int companyId, string sourceFolder)
        {
            string companyDirPathName = $"{CvsRootFolder}\\{companyId}_";
            List<cv> cvsIds = await _cvsPositionsQueries.GetCompanyCvs(companyId);

            var companyDir = new DirectoryInfo(sourceFolder);

            FileInfo[] files = companyDir.GetFiles("*.*", SearchOption.AllDirectories);

            if (!Directory.Exists(companyDirPathName))
            {
                Directory.CreateDirectory(companyDirPathName);
            }

            foreach (var file in files)
            {
                if (file.Name.Substring(0, 3) == "cv_")
                {
                    var cvId = Convert.ToInt32(file.Name.Substring(3, file.Name.IndexOf('.') - 3));
                    var cv = cvsIds.Where(x => x.cvdbid == cvId).FirstOrDefault();
                    if (cv != null)
                    {
                        var fileExtension = file.Extension;
                        var fileMonth = cv.date_created.Month.ToString("00");
                        //var fileMonth = fileNum < 10 ? "0" + fileNum.ToString() : fileNum.ToString();
                        var fileYear = cv.date_created.Year;

                        var cvFolder = $@"{companyDirPathName}\{fileYear}\{fileMonth}";
                        var newFileName = $@"{cvFolder}\{companyId}-{fileYear}{fileMonth}-{cv.id}{fileExtension}";

                        if (!File.Exists(newFileName))
                        {
                            if (!Directory.Exists(cvFolder))
                            {
                                Directory.CreateDirectory(cvFolder);
                            }

                            file.CopyTo(newFileName);
                        }
                    }
                }
            }
        }

        public async void RemoveUnRelatedCvsFiles()
        {

            //security this operation is un reversable
            //return;
            List<int> companiesIds = await _cvsPositionsQueries.GetCompaniesIds();
            DeleteNotRelatedCompaniesFolders(companiesIds);

            foreach (var companyId in companiesIds)
            {
                List<string?> cvsIds = await _cvsPositionsQueries.GetCompanyCvsIds(companyId);
                DeleteNotRelatedCvs(companyId, cvsIds);
            }
        }

        public void DeleteNotRelatedCvs(int companyId, List<string?> cvsIds)
        {
            string companyDirPathName = $"{CvsRootFolder}\\{companyId}_";

            if (Directory.Exists(companyDirPathName))
            {
                var companyDir = new DirectoryInfo(companyDirPathName);

                FileInfo[] files = companyDir.GetFiles("*.*", SearchOption.AllDirectories);

                foreach (var file in files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(file.Name);

                    if (cvsIds != null && cvsIds.IndexOf(fileName) == -1)
                    {
                        file.Delete();
                    }
                }

                DeleteEmptyCvsDirs(companyDirPathName);
            }
        }

        public void DeleteNotRelatedCompaniesFolders(List<int> companiesIds)
        {

            DirectoryInfo di = new DirectoryInfo(CvsRootFolder);
            DirectoryInfo[] arrDir = di.GetDirectories();

            foreach (var dir in arrDir)
            {
                string companyIdFromDirName = dir.Name.Replace("_", "");
                int cId;

                if (int.TryParse(companyIdFromDirName, out cId))
                {
                    if (companiesIds.IndexOf(cId) == -1)
                    {
                        dir.Delete(true);
                    }
                }
            }
        }

        private void DeleteEmptyCvsDirs(string startLocation)
        {
            foreach (var directory in Directory.GetDirectories(startLocation))
            {
                DeleteEmptyCvsDirs(directory);
                if (Directory.GetFiles(directory).Length == 0 &&
                    Directory.GetDirectories(directory).Length == 0)
                {
                    Directory.Delete(directory, false);
                }
            }
        }
    }
}
