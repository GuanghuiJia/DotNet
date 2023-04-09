using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace AspNetCoreWebApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .UseSerilog((_, config) =>
                {
                    config.MinimumLevel.Debug()
                        .Enrich.FromLogContext()
                        .WriteTo.Elasticsearch(new ElasticsearchSinkOptions(new Uri("http://124.223.225.27:9200"))
                        {
                            IndexFormat = "logstash-gh-{0:yyyy.MM}",
                            EmitEventFailure = EmitEventFailureHandling.RaiseCallback,
                            FailureCallback = e => Console.WriteLine("Unable to submit event " + e.MessageTemplate),
                            AutoRegisterTemplate = true,
                            AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                            ModifyConnectionSettings =
                                conn =>
                                {
                                    conn.ServerCertificateValidationCallback((_, _, _, _) => true);
                                    
                                    return conn;
                                }
                        }); 
                });
    }
}
