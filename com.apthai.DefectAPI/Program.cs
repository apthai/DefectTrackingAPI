using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using com.apthai.DefectAPI.Configuration;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Core;
using Serilog.Events;
using Serilog.Formatting.Compact;

namespace com.apthai.DefectAPI
{
    public class Program
    {
        public IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {

            //var config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
            //var config = new ConfigurationBuilder()
            //                .AddJsonFile("appsettings.json", optional: false)
            //                //.Build();
            //AppSettings appsetting = new AppSettings();
            //Configuration.GetSection("AppSettings").Bind(appsetting);

            Log.Logger = new LoggerConfiguration()
                  .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
                  .Enrich.FromLogContext()
                    .WriteTo.ColoredConsole(
                            LogEventLevel.Verbose,
                            "{NewLine}{Timestamp:HH:mm:ss} [{Level}] ({CorrelationToken}) {Message}{NewLine}{Exception}")
                    .WriteTo.File("D:\\Logs\\log.txt",
                           rollingInterval: RollingInterval.Day,
                           fileSizeLimitBytes: 5000000, //5MB
                           rollOnFileSizeLimit: true,
                           retainedFileCountLimit: 31,
                           shared: true
                   )
                 // .ReadFrom.Configuration(configuration)
                 // .WriteTo.File(new CompactJsonFormatter(), "api.log" ,  )
                 .CreateLogger();


            try
            {
                CreateWebHostBuilder(args).Build().Run();
            }
            finally
            {
                Log.CloseAndFlush();
            }

            //var config = new ConfigurationBuilder()
            //      .SetBasePath(Directory.GetCurrentDirectory())
            //      .AddJsonFile("hosting.json", optional: true)
            //      .Build();
            //BuildWebHost(args, config).Run();


        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseIISIntegration()
             .UseSerilog()
             .UseStartup<Startup>()
            .UseKestrel(o => { o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10); });



        //public static IWebHost BuildWebHost(string[] args, IConfiguration config) =>
        //    WebHost.CreateDefaultBuilder(args)
        //                    .UseConfiguration(config)
        //                    //.UseKestrel(e=>e.)
        //                    .UseContentRoot(Directory.GetCurrentDirectory())
        //                    .UseIISIntegration()
        //                    .UseStartup<Startup>()
        //                    .Build();

    }
}
