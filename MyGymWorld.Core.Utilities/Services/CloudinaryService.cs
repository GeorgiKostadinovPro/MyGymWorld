namespace MyGymWorld.Core.Utilities.Services
{
    using CloudinaryDotNet;
    using CloudinaryDotNet.Actions;
    using Microsoft.AspNetCore.Http;
    using MyGymWorld.Core.Utilities.Contracts;
    using System.Threading.Tasks;

    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary cloudinary;

        public CloudinaryService(Cloudinary _cloudinary)
        {
            this.cloudinary = _cloudinary;
        }

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile formFile)
        {
            ImageUploadResult uploadResult = new ImageUploadResult();

            if (formFile.Length > 0)
            {
                using Stream stream = formFile.OpenReadStream();

                ImageUploadParams imageUploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Folder = "MyGymWorld/assets/user-profile-pictures"
                };

                uploadResult = await this.cloudinary.UploadAsync(imageUploadParams);
            }

            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            DeletionParams deletionParams = new DeletionParams(publicId);

            DeletionResult deletionResult = await this.cloudinary.DestroyAsync(deletionParams);

            return deletionResult;
        }
    }
}
