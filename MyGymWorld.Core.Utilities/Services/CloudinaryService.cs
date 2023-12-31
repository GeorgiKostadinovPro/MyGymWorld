﻿namespace MyGymWorld.Core.Utilities.Services
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

        public async Task<ImageUploadResult> UploadPhotoAsync(IFormFile formFile, string folder)
        {
            ImageUploadResult uploadResult = new ImageUploadResult();

            if (formFile.Length > 0)
            {
                using Stream stream = formFile.OpenReadStream();

                ImageUploadParams imageUploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Folder = folder
                };

                uploadResult = await this.cloudinary.UploadAsync(imageUploadParams);
            }

            return uploadResult;
        }

		public async Task<ImageUploadResult> UploadPhotoAsync(byte[] file, string folder)
		{
			byte[] imageBytes = file;
            ImageUploadResult uploadResult;

			using (MemoryStream uploadStream = new MemoryStream(imageBytes))
			{
				ImageUploadParams imageUploadParams = new ImageUploadParams()
				{
					File = new FileDescription(file.ToString(), uploadStream),
                    Folder = folder
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

        public bool IsFileValid(IFormFile formFile)
        {
            if (formFile == null)
            {
                return false;
            }

            string extension = Path.GetExtension(formFile.FileName);

            string[] validExtensions = { ".jpg", ".jpeg", ".png" };

            if (!validExtensions.Contains(extension))
            {
                return false;
            }

            return true;
        }
	}
}
