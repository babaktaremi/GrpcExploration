using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GrpcClientWorkerService.Services.JokeApi;
using RandomMessage.Protos;

namespace GrpcClientWorkerService.Services.GrpcService
{
   public class GrpcClientRandomMessageService
   {
       private readonly RandomMessageService.RandomMessageServiceClient grpcClient;
       private readonly RandomJokeApiService _jokeApiService;

       public GrpcClientRandomMessageService(RandomMessageService.RandomMessageServiceClient grpcClient, RandomJokeApiService jokeApiService)
       {
           this.grpcClient = grpcClient;
           _jokeApiService = jokeApiService;
       }

       public async Task<bool> SendRandomMessage()
       {
           var message = await _jokeApiService.GetRandomJoke();

           var date = DateTimeOffset.Now.ToUnixTimeSeconds();

          var result= await grpcClient.SendRandomMessageAsync(new RanodmMessageContent
               {Message = $"{message.Joke}", SentAt = date});

          return result.IsReceived;
       }

       public async Task<RanodmMessageContent> GetRandomMessage()
       {
           return await grpcClient.GetRandomMessageAsync(new Empty());
       }
   }
}
