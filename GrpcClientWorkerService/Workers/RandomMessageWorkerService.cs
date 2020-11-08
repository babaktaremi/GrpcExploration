using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GrpcClientWorkerService.Services.GrpcService;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GrpcClientWorkerService.Workers
{
   public class RandomMessageWorkerService:BackgroundService
   {
       private readonly ILogger<RandomMessageWorkerService> _logger;
       private readonly IServiceProvider _serviceProvider;
       public RandomMessageWorkerService(ILogger<RandomMessageWorkerService> logger, IServiceProvider serviceProvider)
       {
           _logger = logger;
           _serviceProvider = serviceProvider;
       }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using var scope = _serviceProvider.CreateScope();

                    var service = scope.ServiceProvider.GetRequiredService<GrpcClientRandomMessageService>();

                    await service.SendRandomMessage();

                    await Task.Delay(2000, stoppingToken);

                    var randomJoke = await service.GetRandomMessage();

                    _logger.LogWarning($"New Joke! \n {randomJoke.Message} \n time:{DateTimeOffset.FromUnixTimeMilliseconds(randomJoke.SentAt)}");

                    await Task.Delay(5000, stoppingToken);
                }
                catch (Exception e)
                {
                   _logger.LogError(e.ToString());
                }
            }
        }
    }
}
