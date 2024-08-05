using System.Collections.Generic;

namespace TelemetryLib;

public interface IEmailService
{
    void Send(EmailMessage emailMessage);
    List<EmailMessage> ReceiveEmail(int maxCount = 10);
}
