using System;
using System.Collections.Generic;

namespace TelemetryLib
{
    public class EmailService : IEmailService
    {
        private readonly IEmailConfiguration _emailConfiguration;

        public EmailService(IEmailConfiguration emailConfiguration)
        {
            _emailConfiguration = emailConfiguration;
        }

        public List<EmailMessage> ReceiveEmail(int maxCount = 10)
        {
            throw new NotImplementedException();
        }

        public void Send(EmailMessage emailMessage)
        {

            using (var emailClient = new System.Net.Mail.SmtpClient(_emailConfiguration.SmtpServer, _emailConfiguration.SmtpPort))
            {
                emailClient.UseDefaultCredentials = false;
                emailClient.Credentials = new System.Net.NetworkCredential(_emailConfiguration.SmtpUsername, _emailConfiguration.SmtpPassword);
                emailClient.EnableSsl = true;

                emailClient.Send(emailMessage.ToAddresse.Address, emailMessage.FromAddress.Address, emailMessage.Subject, emailMessage.Content);
            }
        }
    }
}
