using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.FileProviders;
using MysticBox.Models;

namespace MysticBox.Controllers
{
    public class DownloadController : Controller
    {

        [HttpGet]
        public IActionResult ViewImage(string name)
        {
            if (name == null)
                return Content("filename not present");

            name = Path.GetFileNameWithoutExtension(name);

            ImageModel image = IoC.ApplicationDbContext.Images.Find(name);

            if(image == null)
                return Content("invalid filename");
            var num = name.Substring(name.Length - 2);
            var path = Path.Combine(Program.Root, "Data", "Images", num, name);

            Response.Headers.Add("Content-Disposition", "inline; filename=" + image.OriginalName);
            return File(System.IO.File.ReadAllBytes(path), image.ContentType);
        }

        [HttpGet]
        public IActionResult GetDoc(string name)
        {
            //TODO force utf-8
            if (name == null)
                return Content("filename not present");

            name = Path.GetFileNameWithoutExtension(name);

            DocumentModel doc = IoC.ApplicationDbContext.Documents.Find(name);

            if (doc == null)
                return Content("invalid filename");

            var num = name.Substring(name.Length - 2);
            var path = Path.Combine(Program.Root, "Data", "Documents", num, name);
            
            Response.Headers.Add("Content-Disposition", $"inline; filename={doc.OriginalName}");
            return File(System.IO.File.ReadAllBytes(path), doc.ContentType);
        }

        [HttpGet]
        public IActionResult GetAudioVideo(string name)
        {
            if (name == null)
                return Content("filename not present");

            name = Path.GetFileNameWithoutExtension(name);

            AudioVideoModel audioVideo = IoC.ApplicationDbContext.Audio_Videos.Find(name);

            if (audioVideo == null)
                return Content("invalid filename");

            var num = name.Substring(name.Length - 2);
            var path = Path.Combine(Program.Root, "Data", "Audio_Videos", num, name);

            Response.Headers.Add("Content-Disposition", "inline; filename=" + audioVideo.OriginalName);
            Response.Headers.Add("Accept-Ranges", "bytes");
            //Response.Headers.Remove("Cache-Control");


            return File(System.IO.File.ReadAllBytes(path), audioVideo.ContentType);
        }

        [HttpGet]
        public IActionResult Download(string name)
        {
            if (name == null)
                return Content("filename not present");

            name = Path.GetFileNameWithoutExtension(name);

            FileModel file = IoC.ApplicationDbContext.Files.Find(name);
            if (file == null)
                return Content("invalid filename");

            var num = name.Substring(name.Length - 2);
            var path = Path.Combine(Program.Root, "Data", "Files", num, name);

            return File(System.IO.File.ReadAllBytes(path), "application/octet-stream", file.OriginalName);
        }
    }
}