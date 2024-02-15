using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace Tameenk.Services.ServicesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ServicesController : ControllerBase
    {
        [Route("~/api/policy/GetFile")]
        [HttpGet]
        public async Task<IActionResult> GetPolicyFile(string filePath = "default")
        {
            try
            {
                if (string.IsNullOrEmpty(filePath) || filePath == "default")
                {
                    return BadRequest("file path empty");
                }
                return Ok<byte[]>(File.ReadAllBytes(filePath));
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }

        [Route("~/api/policy/uploadFile")]
        [HttpPost]
        public async Task<IActionResult> UploadFile()
        {
            try
            {
                byte[] pdfData = null;
                string filePath = HttpContext.Current.Request.Form[0];
                string directoryPath = HttpContext.Current.Request.Form[1];
                if (HttpContext.Current.Request.Files.Count > 0)
                {
                    HttpPostedFile file = HttpContext.Current.Request.Files[0];
                    MemoryStream target = new MemoryStream();
                    file.InputStream.CopyTo(target);
                    pdfData = target.ToArray();
                }
                if (string.IsNullOrEmpty(filePath) || string.IsNullOrEmpty(directoryPath))
                {
                    return BadRequest("file path empty");
                }
                if (pdfData == null)
                {
                    return BadRequest("pdfData bytes is null");
                }
                if (!Directory.Exists(directoryPath))
                    Directory.CreateDirectory(directoryPath);
                File.WriteAllBytes(filePath, pdfData);
                return Ok<bool>(true);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.ToString());
            }

        }
    }
}
