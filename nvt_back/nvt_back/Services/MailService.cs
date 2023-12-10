using Microsoft.Extensions.Options;
using nvt_back.Services.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Text.RegularExpressions;

namespace nvt_back.Services
{
    public class MailService : IMailService
    {
        private IOptions<EmailSettings> _options;

        public MailService(IOptions<EmailSettings> options)
        {
            this._options = options;
        }

        public async void SendPropertyApprovedEmail(string email, string name, string propertyName)
        {
            string templateId = "d-3204e708408e43a8a6bbeb07838c4b86";

            try
            {
                await this.SendEmailAsync(email, name, propertyName, templateId, "");
                Console.WriteLine("mail sent to " + email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public async Task SendEmailAsync(string email, string name, string property, string templateId, string reason)
        {
            string? fromEmail = _options.Value.SenderEmail;
            string? fromName = _options.Value.SenderName;
            string? apiKey = _options.Value.ApiKey;
            var sendGridClient = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(email);

            object dynamicData;
            if (string.IsNullOrEmpty(reason))
            {
                dynamicData = new { name, property };
            }
            else
            {
                dynamicData = new { name, property, reason };
            }

            var msg = MailHelper.CreateSingleTemplateEmail(from, to, templateId, dynamicData);

            var response = await sendGridClient.SendEmailAsync(msg);
        }

        public async void SendPropertyDeniedEmail(string email, string name, string propertyName, string reason)
        {
            string templateId = "d-e1526454046647518d386ba56f1b0838";

            try
            {
                await this.SendEmailAsync(email, name, propertyName, templateId, reason);
                Console.WriteLine("mail sent to " + email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }
    }
}
