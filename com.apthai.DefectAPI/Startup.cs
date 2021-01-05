using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using com.apthai.CoreApp.API.Base;
using com.apthai.CoreApp.Data.Services;
using com.apthai.DefectAPI.Configuration;
using com.apthai.DefectAPI.Repositories;
using com.apthai.DefectAPI.Repositories.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.Filters;
using com.apthai.DefectAPI.HttpRestModel;
using Hangfire;
using Hangfire.Dashboard;
using Hangfire.Annotations;
using Hangfire.Common;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.Extensions.FileProviders;
using Hangfire.SqlServer;

namespace com.apthai.DefectAPI
{
    public class Startup
    {

        readonly string MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
        public IConfigurationRoot Configuration { get; set; }
        private IHostingEnvironment _hostingEnv;

        public static JsonSerializerSettings jsonCloudSettting = new JsonSerializerSettings()
        {
            DateFormatHandling = DateFormatHandling.IsoDateFormat,
            DateTimeZoneHandling = DateTimeZoneHandling.Local,
            DateParseHandling = DateParseHandling.DateTimeOffset,
            NullValueHandling = NullValueHandling.Ignore,
            ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            Formatting = Formatting.None
        };

        //public Startup(IConfiguration configuration)
        //{
        //    Configuration = configuration;
        //}

        public Startup(IHostingEnvironment env)
        {

            var builder = new ConfigurationBuilder()
               .SetBasePath(env.ContentRootPath)
               .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
               .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
               .AddEnvironmentVariables();

            _hostingEnv = env;

            if (string.IsNullOrWhiteSpace(_hostingEnv.WebRootPath))
            {
                _hostingEnv.WebRootPath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            //Configuration = configuration;
            Configuration = builder.Build();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);


            // needed to load configuration from appsettings.json
            services.AddOptions();

            // needed to store master data && rate limit counters and ip rules
            services.AddMemoryCache();


            // Add framework services.
            //services.AddApplicationInsightsTelemetry(Configuration);
            services.AddCors(options => options.AddPolicy("AllowAll", p => p
                                                                        .AllowAnyOrigin()
                                                                      .AllowAnyMethod()
                                                                       .AllowAnyHeader()
                                                                       .AllowCredentials()
                                                                       ));

            

            services.AddMvc()
              .AddJsonOptions(options =>
              {
                  // options.SerializerSettings.DateFormatString = "yyyy-MM-ddTHH:mm:ss";
                  options.SerializerSettings.ContractResolver = new DefaultContractResolver();
                  options.SerializerSettings.DateFormatHandling = DateFormatHandling.IsoDateFormat;
                  options.SerializerSettings.DateTimeZoneHandling = DateTimeZoneHandling.Local; //RoundtripKind;
                  options.SerializerSettings.DateParseHandling = DateParseHandling.DateTimeOffset;
                  options.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                  options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;


              }).SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            var jsonCloudSettting = new JsonSerializer
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateTimeZoneHandling = DateTimeZoneHandling.Local,
                DateParseHandling = DateParseHandling.DateTimeOffset,
                NullValueHandling = NullValueHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore

            };
            services.AddSingleton(_ => jsonCloudSettting);


            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
            //});

            services.AddSwaggerGen(c =>
            {
                //c.SwaggerDoc("v1", new Info { Title = "My API", Version = "v1" });
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Defect Tracking API Explorer",
                    Description = "Defect Tracking Resful API Document",
                    TermsOfService = "None",
                    Contact = new Contact { Name = "IT Department", Email = "", Url = "it@apthai.com" },
                    License = new License { Name = "IT Department", Url = "https://apthai.com" }
                });
                c.EnableAnnotations();

                var basePath =  PlatformServices.Default.Application.ApplicationBasePath;

                //c.OperationFilter<AddRequiredHeaderParameter>();
                c.DescribeAllEnumsAsStrings();
                var security = new Dictionary<string, IEnumerable<string>>
                {
                    {"Bearer", new string[] { }},
                };
                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = "header",
                    Type = "apiKey"
                });
                c.AddSecurityRequirement(security);
                c.SchemaRegistryOptions.CustomTypeMappings.Add(typeof(IFormFile), () => new Schema() { Type = "file", Format = "binary" });
                // c.OperationFilter<ExamplesOperationFilter>();

                // [SwaggerRequestExample] & [SwaggerResponseExample]
                // version < 3.0 like this: c.OperationFilter<ExamplesOperationFilter>(); 
                // version 3.0 like this: c.AddSwaggerExamples(services.BuildServiceProvider());
                // version 4.0 like this:
                //c.ExampleFilters();


                c.OperationFilter<AddFileParamTypesOperationFilter>(); // Adds an Upload button to endpoints which have [AddSwaggerFileUploadButton]
                                                                       // c.OperationFilter<AddHeaderOperationFilter>("correlationId", "Correlation Id for the request", false); // adds any string you like to the request headers - in this case, a correlation id
                c.OperationFilter<AddResponseHeadersFilter>(); // [SwaggerResponseHeader]




            });

            var conn = Environment.GetEnvironmentVariable("DefaultConnection");
            if (conn == null)
            {
                conn = Configuration.GetConnectionString("DefaultConnection");
            }
            var HangFireOption = new SqlServerStorageOptions
            {
                CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
                SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
                QueuePollInterval = TimeSpan.Zero,
                UseRecommendedIsolationLevel = true,
                UsePageLocksOnDequeue = true,
                DisableGlobalLocks = true
            };
            services.AddHangfire(config =>
                config.UseSqlServerStorage(conn, HangFireOption));


            //services.AddSwaggerExamples();

            //services.AddSwaggerExamplesFromAssemblyOf<ParamSyncUnitModel>();


            services.AddTransient<IMasterRepository, MasterRepository>();


            services.AddSingleton<IConfiguration>(Configuration);
            var setting = Configuration.GetSection("AppSettings").Get<AppSettings>();
            services.AddSingleton(setting);



            services.AddSingleton(_hostingEnv);
            services.AddSingleton<IAuthorizeService, AuthorizeService>();
            services.AddSingleton<IDataCrawlerServices, DataCrawlerServices>();
            services.AddSingleton<IUnitOfWork, UnitOfWork>();
            services.AddSingleton<IUserRepository, UserRepository>();
            services.AddSingleton<ISyncRepository, SyncRepository>();
            services.AddSingleton<ITransactionRepository, TransactionRepository>();
            //services.AddSingleton<IFacilitiesTransactionsRepository, FacilitiesTransactionsRepository>();

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IConfiguration configuration, AppSettings settings, ILogger<Startup> logger)
        {
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }


            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            app.UseStaticFiles();

            app.UseHangfireDashboard();

            app.UseHangfireServer();


            app.UseHangfireDashboard("/Defectjobs", new DashboardOptions()
            {
                Authorization = new[] { new HangFireAuthorizationFilter() }
            });

            var name = settings.BaseRootPath;
            var endpoint = !string.IsNullOrEmpty(name) ?
                string.Format("/{0}/swagger/v1/swagger.json", name) :
                    "/swagger/v1/swagger.json";

            //var name = 
            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                // var URLS = app.ServerFeatures.Get<IServerAddressesFeature>().Addresses;

                c.SwaggerEndpoint(endpoint, "Defect2020 API V1");
                c.ShowExtensions();
                c.RoutePrefix = "docs";

            });

            app.UseHttpsRedirection();
            app.UseCors(MyAllowSpecificOrigins);
            app.UseMvc();


            UtilsProvider.Config = Configuration;
            UtilsProvider.HostingEnvironment = env;
            UtilsProvider.AppSetting = settings;


            var manager = new RecurringJobManager();
            //manager.AddOrUpdate("core-daily_sync_master", Job.FromExpression(() => RecurringJobDaily_Master()), Cron.Daily(23 - 2, 30)); // 2 AM
            manager.AddOrUpdate("TestHangFireJobs", Job.FromExpression(() => RecurringJobTestHangFire_Master()), Cron.Minutely()); // 2 AM
            //manager.AddOrUpdate("TestHangFireJobs_2", Job.FromExpression(() => RecurringJobTestHangFire_Master()), Cron.MinuteInterval(4)); // 2 AM
            ////var manager = new RecurringJobManager();
            //manager.AddOrUpdate("core-daily_sync_master", Job.FromExpression(() => RecurringJobDaily_Master()), Cron.Daily(2-4)); // 2 AM

            // run now on startup
            // BackgroundJob.Enqueue(() => RecurringJobDaily_Master());



        }

        //public static async Task RecurringJobDaily_Master()
        //{
        //    try
        //    {
        //        Console.WriteLine("RecurringJobDaily_Master Running.. at " + DateTime.Now.ToString() + " " + TimeZoneInfo.Local.ToString());

        //        var repSync = new SyncRepository(UtilsProvider.HostingEnvironment, UtilsProvider.Config);

        //        await repSync.ReplicateMasterWebToMobileAsync();
        //    } 
        //    catch (Exception ex)
        //    {
        //        while (ex.InnerException != null)
        //            ex = ex.InnerException;

        //        var ilog = UtilsProvider.ApplicationLogging.CreateLogger<Startup>();
        //        ilog.LogError("RecurringJobDaily_Master Error :: " + ex.Message);

        //        throw ex;


        //    }



        //}

        public static async Task RecurringJobTestHangFire_Master()
        {
            try
            {
                Console.WriteLine("RecurringJobTestHangFire_Master Running.. at " + DateTime.Now.ToString() + " " + TimeZoneInfo.Local.ToString());

                var repSync = new SyncRepository(UtilsProvider.HostingEnvironment, UtilsProvider.Config);

                await repSync.TanonchaiJobSample();
            }
            catch (Exception ex)
            {
                while (ex.InnerException != null)
                    ex = ex.InnerException;

                var ilog = UtilsProvider.ApplicationLogging.CreateLogger<Startup>();
                ilog.LogError("RecurringJobDaily_Master Error :: " + ex.Message);

                throw ex;

            }
        }
    }

    public class HangFireAuthorizationFilter : IDashboardAuthorizationFilter
    {
        public bool Authorize([NotNull] DashboardContext context)
        {

            //can add some more logic here...
            //return HttpContext.Current.User.Identity.IsAuthenticated;
            return true;

        }
    }


}
