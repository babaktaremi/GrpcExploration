using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Grpc.Net.Client;
using GrpcClientWorkerService.Services.FileService;
using GrpcClientWorkerService.Services.GrpcService;
using GrpcClientWorkerService.Services.JokeApi;
using GrpcClientWorkerService.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RandomMessage.Protos;

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
                    services.AddScoped<GrpcFileStreamingClientService>();

                    services.AddHttpClient<RandomJokeApiService>();

                    services.AddScoped<GrpcClientRandomMessageService>(sp =>
                    {
                        var serverAddress=hostContext.Configuration.GetSection("Grpc").GetChildren()
                            .Where(c => c.Key.Equals("ServerUrl"))
                            .Select(c => c.Value).FirstOrDefault();

                        var apiService = sp.GetRequiredService<RandomJokeApiService>();

                        var channel = GrpcChannel.ForAddress(serverAddress);

                        return new GrpcClientRandomMessageService(new RandomMessageService.RandomMessageServiceClient(channel), apiService);
                    });

                   // services.AddHostedService<FileSenderClient>();

                   services.AddHostedService<RandomMessageWorkerService>();
                });
    }
}
