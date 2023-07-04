namespace MyGymWorld.Core.Utilities.Services
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using MyGymWorld.Core.Utilities.Contracts;
    using System;
    using System.Threading.Tasks;

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinary;

        public CloudinaryService(Cloudinary _cloudinary)
        {
            this.cloudinary = _cloudinary;
        }
        public Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            throw new NotImplementedException();
        }

        public Task<string> UploadPhotoAsync(IFormFile formFile)
        {
            throw new NotImplementedException();
        }
    }
}
