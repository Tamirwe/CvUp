using GeneralLibrary;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Cryptography;
using System.Text;

namespace CvUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DownloadController : ControllerBase
    {
        [HttpGet]
        public string Get()
        {
            string path = @"C:\CvUpCvs\132_\2022_\11_\132_2022_11_26_1648_2041.pdf";


            Byte[] bytes = System.IO.File.ReadAllBytes(path);
            String fileBase64 = Convert.ToBase64String(bytes);
            return fileBase64;

            //if (System.IO.File.Exists(path))
            //{
            //    return File(System.IO.File.OpenRead(path), "application/octet-stream");
            //}
            //return NotFound();
        }

     


        [HttpGet]
        [Route("GetJpg")]
        public IActionResult GetJpg()
        {

            string path1 = @"C:\CvUpCvs\132_\2022_\11_\ggg.jpg";
            return PhysicalFile(path1, "image/jpg", true);
        }

        [HttpGet]
        [Route("GetWord")]
        public IActionResult GetWord()
        {
            string path = @"C:\CvUpCvs\132_\2022_\11_\132_2022_11_26_1648_1981.docx";
            return PhysicalFile(path, "application/msword", true);
        }

        [HttpGet]
        [Route("GetWord2")]
        public IActionResult GetWord2()
        {
            string path = @"C:\CvUpCvs\132_\2022_\11_\cv_57064.doc";

            return PhysicalFile(path, "application/msword", true);
        }

        //[HttpGet]
        //[Route("GetCv14")]
        //public IActionResult GetCv14(string pp)
        //{
        //    string encryptedUserName = "";

        //    if (pp=="a")
        //    {
        //        encryptedUserName = Encriptor.Encrypt(@"C:\CvUpCvs\132_\2022_\11_\cv_57064.doc");
        //    }
        //    else
        //    {
        //        encryptedUserName = Encriptor.Encrypt(@"C:\CvUpCvs\132_\2022_\11_\132_2022_11_26_1648_1981.docx");

        //    }

        //    string decryptedUserName = Encriptor.Decrypt(encryptedUserName);


        //    //string path = @"C:\CvUpCvs\132_\2022_\11_\132_2022_11_26_1648_2041.pdf";
        //    //return PhysicalFile(path, "application/pdf", true);
        //    //string path = @"C:\CvUpCvs\132_\2022_\11_\cv_57064.doc";

        //    return PhysicalFile(decryptedUserName, "application/msword", true);
        //    //return Ok("dfgdfg");
        //}

       
    }
}
