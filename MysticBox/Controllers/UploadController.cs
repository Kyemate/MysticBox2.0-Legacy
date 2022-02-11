using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MysticBox.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MysticBox.Controllers
{
    public class UploadController : Controller
    {
        [HttpPost]
        public ActionResult Progress()
        {
            return this.Content(Startup.Progress.ToString());
        }

        [HttpPost]
        public async Task<IActionResult> AnonUpload(IFormFile file)
        {
            if (file != null || file.Length > 0)
            {
                string publicUploadKey = "PUB_UPLOAD_V1";
                string[] imageTypes = new string[] { "image/jpeg", "image/jp2", "image/webp", "image/gif",
                    "image/png", "image/bmp", "image/x-icon", "image/ico" };

                string fileUrl = Program.Url + "/u/" + await SaveFile(publicUploadKey, file, MimeType.Images);

                return Redirect(fileUrl);
            }
            else
                return Content("Error: there was a problem uploading your file");
        }

        [HttpPost]
        public async Task<IActionResult> Upload(string key, IFormFile files, string returnUrl = null)
        {
            

            if (Program.ValidateUploadKey(key))
            {
                if (files != null || files.Length > 0)
                {

                    string[] imageTypes = new string[] { "image/jpeg", "image/jp2", "image/webp", "image/gif",
                        "image/png", "image/bmp", "image/x-icon", "image/ico" };

                    string[] DocTypes = new string[] { "text/plain", "text/html", "application/pdf", "application/msword",
                        "application/vnd.openxmlformats-officedocument.wordprocessingml.document" };

                    string[] audioVideoTypes = new string[] { "video/mp4", "video/webm", "audio/mpeg", "audio/webm" };

                    string fileUrl;

                    if (imageTypes.Contains(files.ContentType.ToLower()))
                        fileUrl = Program.Url + "/u/" + await SaveFile(key, files, MimeType.Images);

                    else if (DocTypes.Contains(files.ContentType.ToLower()))
                        fileUrl = Program.Url + "/doc/" + await SaveFile(key, files, MimeType.Documents);

                    else if(audioVideoTypes.Contains(files.ContentType.ToLower()))
                        fileUrl = Program.Url + "/va/" + await SaveFile(key, files, MimeType.Audio_Videos);
                    else
                        fileUrl = Program.Url + "/File/" + await SaveFile(key, files, MimeType.Files);


                    if(returnUrl == null)
                        return Content(fileUrl);

                    return Redirect(returnUrl);
                }
                else
                    return Content("Error: there was a problem uploading your file");
            }
            else if (string.IsNullOrEmpty(key))
                return Redirect("http://google.de");
            else
                return StatusCode(403);
            //return Content("Sorry, there was a problem uploading your file. (Ensure your directory has 1337 permissions)");
        }

        private async Task<string> SaveFile(string key, IFormFile file, MimeType mimeType)
        {
            Random rnd = new Random();
            string num = rnd.Next(0, 100).ToString("D2");
            var fileName = Program.RandomString(8) + num; //Path.GetExtension(file.FileName)
            var path = Path.Combine(Program.Root + "/Data/", mimeType.ToString(), num);

           // while (System.IO.File.Exists(Path.Combine(path, num, fileName)))
           //     fileName = Program.RandomString(8) + num;

            path = Path.Combine(Program.Root + "/Data/", mimeType.ToString(), num, fileName);
            Startup.Progress = 0;

            long totalBytes = file.Length;

            //using (var stream = new FileStream(path, FileMode.Create))
            //{
            //    long totalReadBytes = 0;
            //    int readBytes;
            //    Startup.Progress = (int)((float)totalReadBytes / (float)totalBytes * 100.0);

            //    await file.CopyToAsync(stream);
            //}
            byte[] buffer = new byte[16 * 1024];
            using (FileStream output = System.IO.File.Create(path))
            {
                using (Stream input = file.OpenReadStream())
                {
                    long totalReadBytes = 0;
                    int readBytes;

                    while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        await output.WriteAsync(buffer, 0, readBytes);
                        totalReadBytes += readBytes;
                        Startup.Progress = (int)((float)totalReadBytes / (float)totalBytes * 100.0);
                        //await Task.Delay(10); // It is only to make the process slower
                    }
                }
            }


            #region Save To Database
            if (mimeType == MimeType.Images)
                IoC.ApplicationDbContext.Add(new ImageModel
                {
                    UploadKey = key,
                    OriginalName = file.FileName,
                    Name = fileName,
                    Size = file.Length,
                    UploadDate = DateTime.Now,
                    ContentType = file.ContentType
                });
            else if (mimeType == MimeType.Documents)
                IoC.ApplicationDbContext.Add(new DocumentModel
                {
                    UploadKey = key,
                    OriginalName = file.FileName,
                    Name = fileName,
                    Size = file.Length,
                    UploadDate = DateTime.Now,
                    ContentType = file.ContentType
                });
            else if (mimeType == MimeType.Audio_Videos)
                IoC.ApplicationDbContext.Add(new AudioVideoModel
                {
                    UploadKey = key,
                    OriginalName = file.FileName,
                    Name = fileName,
                    Size = file.Length,
                    UploadDate = DateTime.Now,
                    ContentType = file.ContentType
                });
            else if (mimeType == MimeType.Files)
                IoC.ApplicationDbContext.Add(new FileModel
                {
                    UploadKey = key,
                    OriginalName = file.FileName,
                    Name = fileName,
                    Size = file.Length,
                    UploadDate = DateTime.Now
                });

            IoC.ApplicationDbContext.SaveChanges();
            #endregion

            return fileName + Path.GetExtension(file.FileName);
        }
    }
}