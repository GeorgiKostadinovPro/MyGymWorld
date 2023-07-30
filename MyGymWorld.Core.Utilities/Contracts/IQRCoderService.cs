namespace MyGymWorld.Core.Utilities.Contracts
{
    public interface IQRCoderService
    {
        string GenerateQRCodeAsync(string content);
    }
}
