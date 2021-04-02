using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Events;
using System;

namespace com.apthai.DefectAPI
{
    public class Program
    {
        public IConfigurationRoot Configuration { get; set; }
        public static void Main(string[] args)
        {
            //CreateHostBuilder(args).Build().Run();

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

        }     

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseStartup<Startup>()
            .UseKestrel(o => { 
                o.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(10);
                o.Limits.MaxRequestBodySize = 30 * 1024 * 1024;
                o.Limits.RequestHeadersTimeout  = TimeSpan.FromMinutes(10);
            });

    }
}
