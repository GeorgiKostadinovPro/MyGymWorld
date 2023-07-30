namespace MyGymWorld.Core.Utilities.Services
{
    using QRCoder;
    using MyGymWorld.Core.Utilities.Contracts;
	using CloudinaryDotNet.Actions;
	using Microsoft.Extensions.Configuration;

	public class QRCoderService : IQRCoderService
    {
        private readonly ICloudinaryService cloudinaryService;

        private readonly IConfiguration configuration;

        public QRCoderService(ICloudinaryService _cloudinaryService, IConfiguration _configuration)
        {
            this.cloudinaryService = _cloudinaryService;

            this.configuration = _configuration;
        }

        public async Task<(string, string)> GenerateQRCodeAsync(string content)
        {

            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qRCodeGenerator.CreateQrCode($"{this.configuration["ApplicationUrl"]}/Membership/Details?membershipId={content}", QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArray = qrCode.GetGraphic(20);


			ImageUploadResult result = await this.cloudinaryService.UploadPhotoAsync(qrCodeAsBitmapByteArray, "MyGymWorld/assets/user-barcode-pictures");

            return (result.SecureUri.AbsoluteUri, result.PublicId);
        }
    }
} 
