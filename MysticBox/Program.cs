using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MysticBox
{
    public class Program
    {
        public static string Url { get; } = "http://dev.mdpn.net";

        public static string Root { get; private set; }

        public static string WWWRoot { get; private set; }

        private static List<string> uploadKey = new List<string>();


        /// <summary>
        /// Validate Uploadkey
        /// </summary>
        /// <param name="key"></param>
        /// <returns>bool</returns>
        public static bool ValidateUploadKey(string key)
        {
            if (uploadKey.Contains(key))
                return true;
            //var usr = IoC.ApplicationDbContext.Users.Find(b => b.UploadKey.Equals(key));
            var usr = IoC.ApplicationDbContext.Users.FirstOrDefault(b => b.UploadKey.Equals(key));
            if (usr?.UploadKey == key)
            {
                uploadKey.Add(usr.UploadKey);
                return true;
            }
            return false;
        }

        public static void Main(string[] args)
        {
            //System.Diagnostics.Debugger.Launch();
            Root = Directory.GetCurrentDirectory();
            WWWRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
           //.UseSetting("detailedErrors", "true")
           .UseIISIntegration()
           .UseStartup<Startup>()
           //.CaptureStartupErrors(true)
           ;

        public static string RandomString(int length)
        {
            //abcdefghijklmnopqvstuvwz
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
