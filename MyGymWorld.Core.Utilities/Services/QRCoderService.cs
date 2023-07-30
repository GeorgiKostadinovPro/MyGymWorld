namespace MyGymWorld.Core.Utilities.Services
{
    using QRCoder;
    using MyGymWorld.Core.Utilities.Contracts;

    public class QRCoderService : IQRCoderService
    {
        public string GenerateQRCodeAsync(string content)
        {

            QRCodeGenerator qRCodeGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qRCodeGenerator.CreateQrCode(content, QRCodeGenerator.ECCLevel.Q);
            BitmapByteQRCode qrCode = new BitmapByteQRCode(qrCodeData);
            byte[] qrCodeAsBitmapByteArray = qrCode.GetGraphic(20);

            string base64QrCode = Convert.ToBase64String(qrCodeAsBitmapByteArray);

            return base64QrCode;
        }
    }
} 
