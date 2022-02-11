using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MysticBox.Data;
using System;

namespace MysticBox
{
    /// <summary>
    /// A shorthand access class to get DI services with nice clean short code
    /// </summary>
    public static class IoC
    {
        /// <summary>
        /// The scoped instance of the <see cref="AppDbContext"/>
        /// </summary>
        public static AppDbContext ApplicationDbContext => IoCContainer.Provider.GetService<AppDbContext>();
    }

    /// <summary>
    /// The dependency injection container making use of the built in .Net Core service provider
    /// </summary>
    public static class IoCContainer
    {
        /// <summary>
        /// The service provider for this application
        /// </summary> //IServiceProvider
        public static IServiceProvider Provider { get; set; }



        public static IConfiguration Configuration { get; set; }



    }
}