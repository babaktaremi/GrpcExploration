using GrpcClientWorkerService.Services.Dtos;

namespace GrpcClientWorkerService.Services.FileService
{
   public interface IFileNumberService
   {
       FileDto GenerateImage(int width, int height);

   }
}
