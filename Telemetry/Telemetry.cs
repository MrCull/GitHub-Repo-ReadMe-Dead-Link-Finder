using System;
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
            if (_telemetryConfig.TelemetryEnabled)
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
        }
    }
}
