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

        public void SendPropertyApprovedEmail(string email, string name, string propertyName)
        {
            string subject = "Hooray! Your property has been approved";
            string htmlMessage = @"
                                <!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                    <meta charset=""UTF-8"">
                                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                    <style>
                                        body {
                                            font-family: 'Arial', sans-serif;
                                            line-height: 1.6;
                                            color: #333;
                                        }

                                        .container {
                                            max-width: 600px;
                                            margin: 0 auto;
                                        }

                                        .header {
                                            background-color: #f8f8f8;
                                            padding: 20px;
                                            text-align: center;
                                        }

                                        .content {
                                            padding: 20px;
                                        }

                                        .footer {
                                            background-color: #f8f8f8;
                                            padding: 10px;
                                            text-align: center;
                                        }

                                        .approved {
                                            color: green;
                                        }
                                    </style>
                                </head>
                                <body>
                                    <div class=""container"">
                                        <div class=""header"">
                                            <h2>LaCasaDeSmart</h2>
                                        </div>
                                        <div class=""content"">
                                            <p>Dear " + name + @",</p>
                                            <p>Your request to register property with name <strong>" + propertyName + @"</strong> has been <span class=""approved"">APPROVED</span>.</p>
                                            <p>Feel free to log in and start connecting it to your smart devices.</p>
                                            <p>Best,<br>LaCasaDeSmart team</p>
                                        </div>
                                        <div class=""footer"">
                                            <p>&copy; 2023 LaCasaDeSmart. All rights reserved.</p>
                                        </div>
                                    </div>
                                </body>
                                </html>
                                ";

            try
            {
                this.SendEmailAsync(email, subject, htmlMessage).Wait();
                Console.WriteLine("mail sent to " + email);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            string? fromEmail = _options.Value.SenderEmail;
            string? fromName = _options.Value.SenderName;
            string? apiKey = _options.Value.ApiKey;
            var sendGridClient = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(email);
            var plainTextContent = Regex.Replace(htmlMessage, "<[^>]*>", "");
            var msg = MailHelper.CreateSingleEmail(from, to, subject,
            plainTextContent, htmlMessage);
            var response = await sendGridClient.SendEmailAsync(msg);
        }

        public void SendPropertyDeniedEmail(string email, string name, string propertyName, string reason)
        {
            string subject = "Oh no! Your property has been denied";
            string htmlMessage = @"
                                <!DOCTYPE html>
                                <html lang=""en"">
                                <head>
                                    <meta charset=""UTF-8"">
                                    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
                                    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
                                    <style>
                                        body {
                                            font-family: 'Arial', sans-serif;
                                            line-height: 1.6;
                                            color: #333;
                                        }

                                        .container {
                                            max-width: 600px;
                                            margin: 0 auto;
                                        }

                                        .header {
                                            background-color: #f8f8f8;
                                            padding: 20px;
                                            text-align: center;
                                        }

                                        .content {
                                            padding: 20px;
                                        }

                                        .footer {
                                            background-color: #f8f8f8;
                                            padding: 10px;
                                            text-align: center;
                                        }

                                        .approved {
                                            color: red;
                                        }
                                    </style>
                                </head>
                                <body>
                                    <div class=""container"">
                                        <div class=""header"">
                                            <h2>LaCasaDeSmart</h2>
                                        </div>
                                        <div class=""content"">
                                            <p>Dear " + name + @",</p>
                                            <p>Your request to register property with name <strong>" + propertyName + @"</strong> has been <span class=""approved"">DENIED</span>.</p>
                                            <p>Our admins said this was the reason: </p>
                                            <p>" + reason + @"</p>
                                            <p>Best,<br>LaCasaDeSmart team</p>
                                        </div>
                                        <div class=""footer"">
                                            <p>&copy; 2023 LaCasaDeSmart. All rights reserved.</p>
                                        </div>
                                    </div>
                                </body>
                                </html>
                                ";

            try
            {
                this.SendEmailAsync(email, subject, htmlMessage).Wait();
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
