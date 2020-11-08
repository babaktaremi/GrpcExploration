using System;
using System.Threading;
using System.Threading.Tasks;
using GrpcClientWorkerService.Services.GrpcService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrpcClientWorkerService.Workers
{
    public class FileSenderClient : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<FileSenderClient> _logger;
        private readonly IConfiguration _configuration;

        public FileSenderClient(IServiceProvider serviceProvider, ILogger<FileSenderClient> logger, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
            _configuration = configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();


            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var service = scope.ServiceProvider.GetRequiredService<GrpcFileStreamingClientService>();

                    await service.SendMessage(stoppingToken);

                    await Task.Delay(2000, stoppingToken);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

       
    }
}
