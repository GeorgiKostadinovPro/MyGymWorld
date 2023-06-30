namespace MyGymWorld.Core.Utilities.Contracts
{
    using System.Threading.Tasks;

    public interface IEmailSenderService
    {
        Task SendEmailAsync(string toEmail, string subject, string content);
    }
}
