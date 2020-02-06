using com.apthai.DefectAPI.Configuration;
using com.apthai.DefectAPI.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace com.apthai.DefectAPI
{
    public static class UtilsProvider
    {

        public static IHostingEnvironment HostingEnvironment { get; set; }
        public static IConfiguration Config { get; set; }

        public static AppSettings AppSetting { get; set; }



        /// <summary>
        /// Shared logger
        /// </summary>
        internal static class ApplicationLogging
        {
            internal static ILoggerFactory LoggerFactory { get; set; }// = new LoggerFactory();
            internal static ILogger CreateLogger<T>() => LoggerFactory.CreateLogger<T>();
            internal static ILogger CreateLogger(string categoryName) => LoggerFactory.CreateLogger(categoryName);

        }

    }
}
