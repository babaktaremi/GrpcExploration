using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace GrpcClientWorkerService.Services.Dtos
{
   public class RandomJokeDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }
      
        [JsonPropertyName("joke")]
        public string Joke { get; set; }
        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
