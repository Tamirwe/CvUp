using DataModelsLibrary.Queries;
using LuceneLibrary;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvsPositionsLibrary.CvsFiles
{
    public class CvsFilesService : ICvsFilesService
    {
        private ICvsPositionsQueries _cvsPositionsQueries;
        string CvsRootFolder;

        public CvsFilesService(IConfiguration config, ICvsPositionsQueries cvsPositionsQueries)
        {
            _cvsPositionsQueries = cvsPositionsQueries;
            CvsRootFolder = $"{config["GlobalSettings:CvsFilesRootFolder"]}";
        }

        public void RemoveUnRelatedCvsFiles()
        {

            //security this operation is un reversable
            //return;
            List<int> companiesIds = _cvsPositionsQueries.GetCompaniesIds();
            DeleteNotRelatedCompaniesFolders(companiesIds);

            foreach (var companyId in companiesIds)
            {
                List<string?> cvsIds = _cvsPositionsQueries.GetCompanyCvsIds(companyId);
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

        private  void DeleteEmptyCvsDirs(string startLocation)
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
