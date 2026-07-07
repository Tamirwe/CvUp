using Database.models;
using DataModelsLibrary.Models;
using DataModelsLibrary.Queries;
using GeneralLibrary;
using Microsoft.Extensions.Configuration;
using Spire.Pdf;
using Spire.Pdf.Graphics;

namespace CvFilesLibrary
{
    public class CvsFilesService : ICvsFilesService
    {
        private IPositionsQueries _cvsPositionsQueries;
        string _filesRootFolder;

        public CvsFilesService(IConfiguration configuration, IPositionsQueries cvsPositionsQueries)
        {
            _filesRootFolder = configuration["CVS_ROOT_FOLDER"];
            _cvsPositionsQueries = cvsPositionsQueries;
        }

        public CvFileDetailsModel GetCvFileDetails(string cvKey)
        {
            string[] secArr = cvKey.Split("-");
            string companyFolder = secArr[0];
            string yearFolder = secArr[1].Substring(0, 4);
            string monthFolder = secArr[1].Substring(4, secArr[1].Length - 5);
            string fileType = Utils.FileTypeName(secArr[1].Last());
            string fileName = $"{companyFolder}-{yearFolder}{monthFolder}-{secArr[2]}{fileType}";
            string[] pathArr = secArr[0].Split("_");
            string path = $"{_filesRootFolder}\\_{companyFolder}\\cvs\\{yearFolder}\\{monthFolder}\\{fileName}";
            return new CvFileDetailsModel { cvFilePath = path, cvFileType = fileType, fileName= fileName };
        }

        public MemoryStream AddPdfLogo(int companyId, string cvKey)
        {
            CvFileDetailsModel cvFileDetails = GetCvFileDetails(cvKey);

            PdfDocument pdf;

            if (cvFileDetails.cvFileType == ".pdf")
            {
                pdf = new PdfDocument();
                pdf.LoadFromFile(cvFileDetails.cvFilePath);
            }
            else
            {
                Spire.Doc.Document document = new Spire.Doc.Document(cvFileDetails.cvFilePath);
                var stream = new MemoryStream();
                document.SaveToFile(stream, Spire.Doc.FileFormat.PDF);
                stream.Seek(0, SeekOrigin.Begin);
                pdf = new PdfDocument(stream);

                // Remove Spire's evaluation notice page (appended as the last page)
                const int evalPageIndex = 3; // 0-based -> "page 4"
                if (pdf.Pages.Count > evalPageIndex)
                {
                    pdf.Pages.RemoveAt(evalPageIndex);
                }
            }

            PdfPageBase page = pdf.Pages[0];

            PdfImage image = PdfImage.FromFile($"{_filesRootFolder}\\_{companyId}\\logos\\logoForCv.png");

            float width = image.Width * 0.50f;
            float height = image.Height * 0.50f;
            float x = 5f;
            float y = 5f;

            page.Canvas.DrawImage(image, x, y, width, height);

            var pdfStream = pdf.SaveToStream(FileFormat.PDF);
            var memoryPdfStream = pdfStream.Cast<MemoryStream>().First();
            memoryPdfStream.Seek(0, SeekOrigin.Begin);
            return memoryPdfStream;
        }

        public async Task ImportNewCvsExternalDisk(int companyId, string sourceFolder)
        {
            string companyDirPathName = $"{_filesRootFolder}\\_{companyId}\\cvs";
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
            string companyDirPathName = $"{_filesRootFolder}\\_{companyId}\\cvs";

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

            DirectoryInfo di = new DirectoryInfo(_filesRootFolder);
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
