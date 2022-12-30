using CvsPositionsLibrary;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DDController : ControllerBase
    {
        private IConfiguration _configuration;

        public DDController(IConfiguration config, ICvsPositionsServise cvsPosService)
        {
            _configuration = config;
        }

        [HttpGet]
        public IActionResult? Get(string id)
        {
            string decripted = GeneralLibrary.Encriptor.Decrypt(id, _configuration["GlobalSettings:cvsEncryptorKey"] ?? "");
            string[] secArr = decripted.Split("~");

            if (Convert.ToDateTime(secArr[1]).Date == DateTime.Now.Date)
            {

                string[] pathArr = secArr[0].Split("_");
                string path = $"{_configuration["GlobalSettings:CvsFilesRootFolder"]}\\{pathArr[0]}_\\{pathArr[1]}_\\{pathArr[2]}_\\{secArr[0]}.{pathArr[6]}";

                if (pathArr[6] == "pdf")
                {
                    return PhysicalFile(path, "application/pdf", true);
                }

                Spire.Doc.Document document = new Spire.Doc.Document(path);
                var stream = new MemoryStream();
                document.SaveToFile(stream, Spire.Doc.FileFormat.PDF);
                //stream.Position = 0;
                stream.Seek(0, SeekOrigin.Begin);
                var pdfFile = File(stream, "application/pdf", $"{id}.pdf");
                return pdfFile;
            }

            return null;
        }

        //[HttpGet]
        //public IActionResult? Get(string id)
        //{
        //    string decripted = GeneralLibrary.Encriptor.Decrypt(id, _configuration["GlobalSettings:cvsEncryptorKey"]??"");
        //    string[] secArr = decripted.Split("~");

        //    if (Convert.ToDateTime(secArr[1]).Date == DateTime.Now.Date)
        //    {
        //        string[] pathArr = secArr[0].Split("_");
        //        string path = $"{_configuration["GlobalSettings:CvsFilesRootFolder"]}\\{pathArr[0]}_\\{pathArr[1]}_\\{pathArr[2]}_\\{secArr[0]}.{pathArr[6]}";

        //        if (pathArr[6] =="pdf")
        //        {
        //            return PhysicalFile(path, "application/pdf", true);
        //        }

        //        //var stream = new FileStream(@"C:\CvUpCvs\132_\2022_\12_\132_2022_12_05_1525_2101docx.docx", FileMode.Open);

        //        Spire.Doc.Document document = new Spire.Doc.Document(path);
        //        var stream = new MemoryStream();
        //        document.SaveToFile(stream, Spire.Doc.FileFormat.PDF);


        //        stream.Position= 0;


        //        //var pdfFile =  File(stream, MediaTypeNames.Application.Pdf, $"bb.pdf");
        //        var pdfFile = File(stream, "application/pdf", $"{secArr[0]}.pdf");
        //        return pdfFile;
        //    }

        //    return null;

        //}
    }
}
