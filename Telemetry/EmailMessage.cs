namespace TelemetryLib;

public class EmailMessage
{
    public EmailAddress ToAddresse { get; set; }
    public EmailAddress FromAddress { get; set; }
    public string Subject { get; set; }
    public string Content { get; set; }
}