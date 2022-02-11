using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using MysticBox.Data;
using MysticBox.Models;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MysticBox.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {

        protected AppDbContext _context;
        protected UserManager<AppUser> _userManager;
        protected SignInManager<AppUser> _signInManager;


        public HomeController(AppDbContext context, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: Default
        public async Task<IActionResult> Index()
        {
            // Make sure we have the database

            //DriveInfo d = new DriveInfo("B");

            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["user"] = user;
            var images = from i in _context.Images where i.UploadKey == user.UploadKey select i;

            return View(images.ToList().OrderByDescending(x => x.UploadDate));
        }

        [Route("Audio_Videos")]
        public async Task<IActionResult> AudioVideos()
        {
            
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["user"] = user;
            var audioVideos = from f in _context.Audio_Videos where f.UploadKey == user.UploadKey select f;

            return View(audioVideos.ToList().OrderByDescending(x => x.UploadDate));
        }

        [Route("Files")]
        public async Task<IActionResult> Files()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["user"] = user;
            var files = from f in _context.Files where f.UploadKey == user.UploadKey select f;

            return View(files.ToList().OrderByDescending(x => x.UploadDate));
        }

        [Route("Documents")]
        public async Task<IActionResult> Documents()
        {
            var user = await _userManager.GetUserAsync(HttpContext.User);
            ViewData["user"] = user;
            var documents = from f in _context.Documents where f.UploadKey == user.UploadKey select f;

            return View(documents.ToList().OrderByDescending(x => x.UploadDate));
        }

        public async Task<IActionResult> Delete(string name, MimeType mimeType)
        {
            //TODO check for vaid uploadkey
            if (name == null || mimeType == MimeType.None)
            {
                return NotFound();
            }
            AppUser user = await _userManager.GetUserAsync(HttpContext.User);
            BaseModel o;
            switch (mimeType)
            {
                case MimeType.Images:
                    o = IoC.ApplicationDbContext.Images.Find(name);
                    if (o?.UploadKey != user.UploadKey)
                        return StatusCode(403);
                    IoC.ApplicationDbContext.Images.Remove((ImageModel)o);
                    System.IO.File.Delete(Path.Combine(Program.Root, "Data", mimeType.ToString(), name));
                    IoC.ApplicationDbContext.SaveChanges();
                    return RedirectToAction("Index");

                case MimeType.Audio_Videos:
                    o = IoC.ApplicationDbContext.Audio_Videos.Find(name);
                    if (o?.UploadKey != user.UploadKey)
                        return StatusCode(403);
                    IoC.ApplicationDbContext.Audio_Videos.Remove((AudioVideoModel)o);
                    System.IO.File.Delete(Path.Combine(Program.Root, "Data", mimeType.ToString(), name));
                    IoC.ApplicationDbContext.SaveChanges();
                    return RedirectToAction("AudioVideos");

                case MimeType.Documents:
                    o = IoC.ApplicationDbContext.Documents.Find(name);
                    if (o?.UploadKey != user.UploadKey)
                        return StatusCode(403);
                    IoC.ApplicationDbContext.Documents.Remove((DocumentModel)o);
                    System.IO.File.Delete(Path.Combine(Program.Root, "Data", mimeType.ToString(), name));
                    IoC.ApplicationDbContext.SaveChanges();
                    return RedirectToAction("Documents");

                case MimeType.Files:
                    o = IoC.ApplicationDbContext.Files.Find(name);
                    if (o?.UploadKey != user.UploadKey)
                        return StatusCode(403);
                    IoC.ApplicationDbContext.Files.Remove((FileModel)o);
                    System.IO.File.Delete(Path.Combine(Program.Root, "Data", mimeType.ToString(), name));
                    IoC.ApplicationDbContext.SaveChanges();
                    return RedirectToAction("Files");

            }
            return NotFound();
        }

        [AllowAnonymous]
        [Route("ini")]
        public IActionResult Create()
        {
            _context.Database.EnsureCreated();
            Directory.CreateDirectory(Path.Combine(Program.Root, "Data"));
            Directory.CreateDirectory(Path.Combine(Program.Root, "Data", "Images"));
            Directory.CreateDirectory(Path.Combine(Program.Root, "Data", "Audio_Videos"));
            Directory.CreateDirectory(Path.Combine(Program.Root, "Data", "Documents"));
            Directory.CreateDirectory(Path.Combine(Program.Root, "Data", "Files"));

            string[] directories = Directory.GetDirectories((Path.Combine(Program.Root, "Data")));
            foreach (string dir in directories)
            {
                for(int i = 0; i < 100; i++)
                {
                    Directory.CreateDirectory(Path.Combine(dir, i.ToString("D2")));
                }
            }
            return Content("Done");
            //return Content($"Private club {HttpContext.User.Identity.Name}", "text/html");
        }
        [AllowAnonymous]
        [Route("inidebug")]
        public IActionResult CreateDebug()
        {
            _context.Database.EnsureCreated();
            //Directory.CreateDirectory(Path.Combine(Program.Root, "Data"));
            //Directory.CreateDirectory(Path.Combine(Program.Root, "Data", "Images"));
            //Directory.CreateDirectory(Path.Combine(Program.Root, "Data", "Audio_Videos"));
            //Directory.CreateDirectory(Path.Combine(Program.Root, "Data", "Documents"));
            //Directory.CreateDirectory(Path.Combine(Program.Root, "Data", "Files"));

            //string[] directories = Directory.GetDirectories((Path.Combine(Program.Root, "Data")));
            //foreach (string dir in directories)
            //{
            //    for (int i = 0; i < 100; i++)
            //    {
            //        Directory.CreateDirectory(Path.Combine(dir, i.ToString()));
            //    }
            //}
            return Content("Done");
            //return Content($"Private club {HttpContext.User.Identity.Name}", "text/html");
        }

    }
}