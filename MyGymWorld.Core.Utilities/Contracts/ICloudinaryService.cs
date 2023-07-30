namespace MyGymWorld.Core.Utilities.Contracts
{
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public interface ICloudinaryService
    {
        Task<ImageUploadResult> UploadPhotoAsync(IFormFile file, string folder);

        Task<ImageUploadResult> UploadPhotoAsync(byte[] file, string folder);

        Task<DeletionResult> DeletePhotoAsync(string publicId);

        bool IsFileValid(IFormFile formFile);
    } 
}