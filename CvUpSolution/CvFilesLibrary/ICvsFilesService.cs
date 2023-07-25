using DataModelsLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvFilesLibrary
{
    public interface ICvsFilesService
    {
        CvFileDetailsModel GetCvFileDetails(string cvEncriptId);
        Task ImportNewCvsExternalDisk(int companyId, string sourceFolder);
        public void RemoveUnRelatedCvsFiles();
        MemoryStream AddPdfLogo(int companyId, string cvKey);
    }
}
