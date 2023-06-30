namespace MyGymWorld.Core.Utilities.Services
{
    using MyGymWorld.Core.Utilities.Contracts;
    using SendGrid.Helpers.Mail;
    using SendGrid;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Configuration;

    public class EmailSenderService : IEmailSenderService
    {
        private readonly IConfiguration configuration;

        public EmailSenderService(IConfiguration _configuration)
        {
            this.configuration = _configuration;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string content)
        {
            var apiKey = this.configuration["SendGrid:APIKey"];
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(this.configuration["SendGrid:Email"], this.configuration["SendGrid:Name"]);
            var to = new EmailAddress(toEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, content, content);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
