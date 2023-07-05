namespace MyGymWorld.Core.Utilities.Contracts
{
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile file);

        Task<DeletionResult> DeletePhotoAsync(string publicId);

        bool IsFileValid(IFormFile formFile);
    } 
}