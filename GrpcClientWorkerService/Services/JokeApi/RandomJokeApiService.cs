using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using GrpcClientWorkerService.Extensions;
using GrpcClientWorkerService.Services.Dtos;
using Microsoft.Extensions.Configuration;

namespace GrpcClientWorkerService.Services.JokeApi
{
   public class RandomJokeApiService
   {
       private readonly HttpClient _client;

       public RandomJokeApiService(HttpClient client, IConfiguration configuration)
       {
           _client = client;
           _client.BaseAddress = new Uri(configuration.GetSection("JokeApi").GetChildren().Where(c => c.Key.Equals("Address"))
               .Select(c => c.Value).FirstOrDefault());
            _client.DefaultRequestHeaders.Accept
                .Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

       public async Task<RandomJokeDto> GetRandomJoke()
       {
           var response = await _client.GetAsync("");

           if (!response.IsSuccessStatusCode)
               return null;

           var content = await response.Content.ReadAsStringAsync();

           return await response.ReadContentAs<RandomJokeDto>();
       }
   }
}
