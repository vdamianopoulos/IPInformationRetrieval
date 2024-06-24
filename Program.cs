using Asp.Versioning;
using IPInformationRetrieval.Abstractions;
using IPInformationRetrieval.Controllers;
using IPInformationRetrieval.Database;
using IPInformationRetrieval.Database.DatabaseContext;
using IPInformationRetrieval.Providers;
using IPInformationRetrieval.Repositories;
using IPInformationRetrieval.Services;
using IPInformationRetrieval.Services.Selectors;
using Microsoft.EntityFrameworkCore;
using Refit;

namespace IPInformationRetrieval
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            ILogger<Program> logger = null;
            try
            {
                var builder = WebApplication.CreateBuilder(args);
                builder.Logging.AddConsole();

                builder.Services.AddControllers();
                builder.Services.AddApiVersioning(options =>
                {
                    options.ReportApiVersions = true;
                    options.ApiVersionReader = new UrlSegmentApiVersionReader();
                    options.AssumeDefaultVersionWhenUnspecified = true;
                    options.DefaultApiVersion = new ApiVersion(1.0);
                })
                .AddMvc();

                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
                builder.Configuration.AddUserSecrets<Program>(optional: true, reloadOnChange: true);

                builder.Services.AddTransient<IIpInformationController, IpInformationController>();
                builder.Services.AddTransient<IIpInformationService, IpInformationService>();
                builder.Services.AddTransient<IIpInformationRepository, IpInformationRepository>();

                builder.Services.AddTransient<IIpDataPersistanceSelectorService, IpDataPersistanceSelectorService>();
                builder.Services.AddTransient<IIpDataRetrievalSelectorService, IpDataRetrievalSelectorService>();
                builder.Services.AddTransient<IIpDataUpdateSelectorService, IpDataUpdateSelectorService>();

                builder.Services.AddTransient<IIpInfoServiceProvider, Ip2cServiceProvider>();

                builder.Services.AddMemoryCache();

                builder.Services.AddSingleton<ICache, Cache.InternalCache>();

                builder.Services.AddSingleton<IDatabaseIpInformation, SqliteDb>();

                builder.Services.AddDbContext<IpInformationDbContext>(options =>
                    options.UseSqlite(builder.Configuration.GetConnectionString("SQLiteDb")), ServiceLifetime.Singleton);

                //Configured hhtpclient to use the resilience pipeline
                builder.Services.AddRefitClient<IIp2cEndpoints>()
                    .ConfigureHttpClient((sp, hc) => hc.BaseAddress = new Uri(builder.Configuration["IpInformationUrl"]))
                    .AddStandardResilienceHandler();

                //This job scheduler workds but sometimes it causes timeout for swagger so i desabled it for now
                //builder.Services.AddHostedService<JobScheduler>();

                //[Obsolete] Not used, only for demonstration purposes
                //builder.Services.AddSingleton<ICache, Cache.RedisCache>();

                //[Obsolete] Not used, only for demonstration purposes
                //builder.Services.AddSingleton<IDatabaseIpInformation, InternalSqlClient>();

                var app = builder.Build();

                var loggerFactory = app.Services.GetService<ILoggerFactory>();
                logger = loggerFactory.CreateLogger<Program>();

                var dbSetup = app.Services.GetService<IDatabaseIpInformation>();
                await dbSetup.SetupAsync();

                if (app.Environment.IsDevelopment())
                {
                    app.UseSwagger();
                    app.UseSwaggerUI();
                }
                app.UseHttpsRedirection();
                app.MapControllers();

                app.Run();

            }
            catch (Exception ex)
            {
                logger?.LogError(ex, ex.Message);
            }
        }
    }
}
