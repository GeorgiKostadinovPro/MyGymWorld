namespace MyGymWorld.Core.Utilities.Contracts
{
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile formFile);

        Task<DeletionResult> DeletePhotoAsync(string publicId);
    } 
}