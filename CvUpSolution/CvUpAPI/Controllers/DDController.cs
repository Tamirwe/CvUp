using CandsPositionsLibrary;
using GeneralLibrary;
using Microsoft.AspNetCore.Mvc;
using Spire.Pdf.Graphics;
using Spire.Pdf;
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

        public DDController(IConfiguration config, ICandsPositionsServise cvsPosService)
        {
            _configuration = config;
        }

        [HttpGet]
        public async Task<IActionResult?> Get(string id)
        {
            //string decripted = GeneralLibrary.Encriptor.Decrypt(id, _configuration["GlobalSettings:cvsEncryptorKey"] ?? "");
            //string[] secArr = decripted.Split("~");

            //if (Convert.ToDateTime(secArr[1]).Date == DateTime.Now.Date)
            //{

            string[] secArr = id.Split("-");

            string companyFolder = secArr[0];
            string yearFolder = secArr[1].Substring(0, 4);
            string monthFolder = secArr[1].Substring(4, secArr[1].Length - 5);
            string fileType = Utils.FileTypeName(secArr[1].Last());
            string fileName = $"{companyFolder}-{yearFolder}{monthFolder}-{secArr[2]}{fileType}";

            string[] pathArr = secArr[0].Split("_");
            string path = $"{_configuration["GlobalSettings:CvsFilesRootFolder"]}\\{companyFolder}_\\{yearFolder}\\{monthFolder}\\{fileName}";

            if (fileType == ".pdf")
            {
                return await Task.Run(() => PhysicalFile(path, "application/pdf", true));
            }

            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile(path);
            var pdfStream = pdf.SaveToStream(FileFormat.PDF);

            var memoryPdfStream = pdfStream.Cast<MemoryStream>().First();//.AsMemory(0);
            memoryPdfStream.Seek(0, SeekOrigin.Begin);
            return await Task.Run(() => File(memoryPdfStream, "application/pdf", $"{id}.pdf"));

            //memoryPdfStream.Seek(0, SeekOrigin.Begin);
            //return await Task.Run(() => File(pdfStream, "application/pdf", $"{id}.pdf"));

            Spire.Doc.Document document = new Spire.Doc.Document(path);
            var stream = new MemoryStream();
            await Task.Run(() => document.SaveToFile(stream, Spire.Doc.FileFormat.PDF));
            //stream.Position = 0;
            stream.Seek(0, SeekOrigin.Begin);
            return await Task.Run(() => File(stream, "application/pdf", $"{id}.pdf"));

        }

        public static void AddPdfLogo()
        {
            //Create a PdfDocument instance
            PdfDocument pdf = new PdfDocument();
            pdf.LoadFromFile("Input.pdf");

            //Get the first page in the PDF document
            PdfPageBase page = pdf.Pages[0];

            //Load an image
            PdfImage image = PdfImage.FromFile("image.jpg");

            //Specify the width and height of the image area on the page
            float width = image.Width * 0.50f;
            float height = image.Height * 0.50f;

            //Specify the X and Y coordinates to start drawing the image
            float x = 180f;
            float y = 70f;

            //Draw the image at a specified location on the page
            page.Canvas.DrawImage(image, x, y, width, height);

            //Save the result document
            pdf.SaveToFile("AddImage.pdf", FileFormat.PDF);
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
