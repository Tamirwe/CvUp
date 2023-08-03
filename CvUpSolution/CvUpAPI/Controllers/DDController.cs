using CandsPositionsLibrary;
using GeneralLibrary;
using Microsoft.AspNetCore.Mvc;
using Spire.Pdf.Graphics;
using Spire.Pdf;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Mime;
using DataModelsLibrary.Models;
using CvFilesLibrary;
using Microsoft.AspNetCore.Authorization;

namespace CvUpAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class DDController : ControllerBase
    {
        private ICvsFilesService _cvsFilesService;

        public DDController(IConfiguration config,  ICvsFilesService cvsFilesService)
        {
            _cvsFilesService = cvsFilesService;
        }

        [HttpGet]
        public async Task<IActionResult?> Get(string id)
        {
            CvFileDetailsModel cvFileDetails = _cvsFilesService.GetCvFileDetails(id);

            if (cvFileDetails.cvFileType == ".pdf")
            {
                return await Task.Run(() => PhysicalFile(cvFileDetails.cvFilePath, "application/pdf", true));
            }

            Spire.Doc.Document document = new Spire.Doc.Document(cvFileDetails.cvFilePath);
            var stream = new MemoryStream();
            await Task.Run(() => document.SaveToFile(stream, Spire.Doc.FileFormat.PDF));
            stream.Seek(0, SeekOrigin.Begin);

            //MemoryStream pdfLogoStream = AddPdfLogo(stream);

            //pdfLogoStream.Seek(0, SeekOrigin.Begin);

            return await Task.Run(() => File(stream, "application/pdf", $"{id}.pdf"));
        }

        //public static MemoryStream AddPdfLogo(MemoryStream stream)
        //{
        //    //Create a PdfDocument instance
        //    PdfDocument pdf = new PdfDocument();
        //    pdf.LoadFromStream(stream);
        //    //pdf.LoadFromFile("Input.pdf");

        //    //Get the first page in the PDF document
        //    PdfPageBase page = pdf.Pages[0];

        //    //Load an image
        //    PdfImage image = PdfImage.FromFile("C:\\GitHub\\CvUp\\CvUpSolution\\CvUpAPI\\Images\\logoForCv.png");

        //    //Specify the width and height of the image area on the page
        //    float width = image.Width * 0.50f;
        //    float height = image.Height * 0.50f;

        //    //Specify the X and Y coordinates to start drawing the image
        //    float x = 5f;
        //    float y = 5f;

        //    //Draw the image at a specified location on the page
        //    page.Canvas.DrawImage(image, x, y, width, height);

        //    var pdfStream = pdf.SaveToStream(FileFormat.PDF);
        //    var memoryPdfStream = pdfStream.Cast<MemoryStream>().First();
        //    stream.Seek(0, SeekOrigin.Begin);
        //    return memoryPdfStream;

        //    /*** FFU  (for future use)****/
        //    //Save the result document
        //    //pdf.SaveToFile("C:\\GitHub\\CvUp\\CvUpSolution\\CvUpAPI\\Images\\AddImage.pdf", FileFormat.PDF);

        //    /***  Delete images ****/
        //    //var pageImages = page.ExtractImages();

        //    //foreach (var item in pageImages)
        //    //{
        //    //    page.DeleteImage(item);
        //    //}
        //    /********************/
        //}

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
