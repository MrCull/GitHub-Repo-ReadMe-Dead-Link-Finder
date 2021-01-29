using System;
using System.IO;
using System.Reflection;

namespace TelemetryLib
{
    public class Telemetry : ITelemetry
    {
        public readonly TelemetryConfig _telemetryConfig;
        public readonly IEmailService _emailService;
        public readonly EmailAddress _emailAddress;

        public Telemetry(TelemetryConfig telemetryConfig, IEmailService emailService, EmailAddress emailAddress)
        {
            _telemetryConfig = telemetryConfig;
            _emailService = emailService;
            _emailAddress = emailAddress;
        }

        public void RecordSearch(string userTelemetryText)
        {
            if (_telemetryConfig.TelemetryEnabledEmail)
            {
                var emailMessage = new EmailMessage()
                {
                    Subject = Assembly.GetExecutingAssembly().GetName().Name + " " + DateTime.UtcNow.ToString(),
                    Content = userTelemetryText,
                    FromAddress = _emailAddress,
                    ToAddresse = _emailAddress
                };

                _emailService.Send(emailMessage);
            }

            if (_telemetryConfig.TelemetryEnabledFile)
            {
                var directory = "telem";
                var fileName = $"{DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss.ffffff")}.txt";
                Directory.CreateDirectory(directory);
                File.AppendAllText($"{directory}\\{fileName}", userTelemetryText);
            }

        }
    }
}
