using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MysticBox.Data;
using MysticBox.Models;
using System;

namespace MysticBox
{
    public class Startup
    {
        public static int Progress { get; set; }

        public Startup(IConfiguration configuration)
        {
            IoCContainer.Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            // Add AppDbContext to DI
            services.AddDbContext<AppDbContext>(options => options.UseSqlServer(IoCContainer.Configuration.GetConnectionString("DefaultConnection")));
            //services.AddDbContext<AppDbContext>(options => options.UseSqlServer("Server=.\\SQLEXPRESS;Database=MysticBoxDev;Trusted_Connection=True;MultipleActiveResultSets=true;"));

            // Adds cookie based authentication
            // Adds scoped classes for thing like UserManager, SignInManager, PasswordHashers
            services.AddIdentity<AppUser, AppRole>().
            //services.AddIdentity<AppUser, IdentityRole>().
               // Adds UserStore and RoleStore from tihs context
               AddEntityFrameworkStores<AppDbContext>().

                // Adds provider that generates unique keys and ashes for thing like
                // forgot password links, phone number verification codes etc....
                AddDefaultTokenProviders();

            #region JWT
            //Add JWT Authentication for api clients
            /*services.AddAuthentication().
                AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = IoCContainer.Configuration["Jwt:Issuer"],
                        ValidAudience = IoCContainer.Configuration["Jwt:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(IoCContainer.Configuration["Jwt:SecretKey"]))
                    };
                });*/
            #endregion

            //Change password policy
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 5;
                options.Password.RequireLowercase = true;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.User.RequireUniqueEmail = true;
            });

            services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/Account/login";

                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
            });

            //services.AddSingleton<IFileProvider>(new PhysicalFileProvider(Path.Combine(Program.WWWRoot, "u")));
            services.AddMvc(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            //services.AddSession();
            //IoC.ApplicationDbContext.Database.EnsureCreated();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            IoCContainer.Provider = serviceProvider;//(ServiceProvider)serviceProvider;

            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            //app.UseHttpsRedirection(); //Redirect http to https
            app.UseStaticFiles();
            //app.UseCookiePolicy();

            app.UseMvc(routes =>
            {//template: "{controller=Home}/{action=Index}/{id?}"
                routes.MapRoute(
                    name: "Default",
                    template: "{controller=Home}/{action=Index}/{id?}");
                routes.MapRoute(
                    name: "Upload",
                    template: "{controller=Upload}/{action=Upload}");

                #region File Access
                routes.MapRoute(
                     name: "u",
                     template: "u/{name}",
                     defaults: new { controller = "Download", action = "ViewImage" });
                routes.MapRoute(
                     name: "va",
                     template: "va/{name}",
                     defaults: new { controller = "Download", action = "GetAudioVideo" });
                routes.MapRoute(
                     name: "doc",
                     template: "doc/{name}",
                     defaults: new { controller = "Download", action = "GetDoc" });
                routes.MapRoute(
                    name: "File",
                    template: "File/{name}",
                    defaults: new { controller = "Download", action = "Download" });
                #endregion
            });

        }
        
    }
}
