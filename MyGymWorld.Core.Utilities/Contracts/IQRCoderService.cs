namespace MyGymWorld.Core.Utilities.Contracts
{
    public interface IQRCoderService
    {
        Task<(string, string)> GenerateQRCodeAsync(string content);
    }
}
