using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GrpcClientWorkerService.Services.FileService;
using GrpcClientWorkerService.Services.GrpcService;
using GrpcClientWorkerService.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GrpcClientWorkerService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddScoped<IFileNumberService, FileNumberService>();
                    services.AddScoped<GrpcClientService>();
                    services.AddHostedService<FileSenderClient>();
                });
    }
}
