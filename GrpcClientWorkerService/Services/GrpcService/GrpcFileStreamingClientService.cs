using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Google.Protobuf;
using Grpc.Core;
using Grpc.Net.Client;
using GrpcClientWorkerService.Services.FileService;
using GrpcExploration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GrpcClientWorkerService.Services.GrpcService
{
    public class GrpcFileStreamingClientService
    {
        private readonly IConfiguration _configuration;
        private readonly IFileNumberService _fileNumberService;
        private readonly ILogger<GrpcFileStreamingClientService> _logger;
        public GrpcFileStreamingClientService(IConfiguration configuration, IFileNumberService fileNumberService, ILogger<GrpcFileStreamingClientService> logger)
        {
            _configuration = configuration;
            _fileNumberService = fileNumberService;
            _logger = logger;
        }

        public async Task SendMessage(CancellationToken stoppingToken)
        {
            var serverAddress = _configuration.GetSection("Grpc").GetChildren()
                .Where(c => c.Key.Equals("ServerUrl"))
                .Select(c => c.Value).FirstOrDefault();

            using var channel = GrpcChannel.ForAddress(serverAddress);

            var client = new RandomNumberFileStreaming.RandomNumberFileStreamingClient(channel);
            using var call = client.StreamFile();

            var file = _fileNumberService.GenerateImage(300, 400).FileByteData;
           
            var size = file.Length / 100;
            byte[] buffer = new byte[size];
            int bytesRead;

            await using Stream source = new MemoryStream(file);

            try
            {

                var t1 = Task.Run(async () =>
                {
                    while ((bytesRead = await source.ReadAsync(buffer, 0, buffer.Length, stoppingToken)) > 0)
                    {

                        await call.RequestStream.WriteAsync(new FileContent{File = ByteString.CopyFrom(buffer)});

                        await Task.Delay(100);
                    }

                    await call.RequestStream.CompleteAsync();
                }, stoppingToken);

                var t2 = Task.Run(async () =>
                {
                    await foreach (var number in call.ResponseStream.ReadAllAsync(cancellationToken: stoppingToken))
                    {
                        _logger.LogInformation($" Progress : {number.Percent} %");
                    }
                }, stoppingToken);

                await Task.WhenAll(t1, t2);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
            finally
            {
                await call.RequestStream.CompleteAsync();
                _logger.LogWarning("Sending Done...");
            }
        }
    }
}
