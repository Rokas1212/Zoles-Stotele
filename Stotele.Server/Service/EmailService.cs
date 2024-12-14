using SendGrid;
using SendGrid.Helpers.Mail;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class EmailService
{
    private readonly string _apiKey;
    private readonly string _senderEmail;
    private readonly string _senderName;

    public EmailService(IConfiguration configuration)
    {
        var sendGridSettings = configuration.GetSection("SendGridSettings");
        _apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY")
                  ?? throw new InvalidOperationException("SENDGRID_API_KEY is not set.");
        _senderEmail = sendGridSettings["SenderEmail"];
        _senderName = sendGridSettings["SenderName"];
    }

    public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string message, string recipientName = "")
    {
        var client = new SendGridClient(_apiKey);
        var from = new EmailAddress(_senderEmail, _senderName);
        var to = new EmailAddress(recipientEmail, recipientName);
        var msg = MailHelper.CreateSingleEmail(from, to, subject, message, message);

        var response = await client.SendEmailAsync(msg);

        return response.StatusCode == System.Net.HttpStatusCode.Accepted;
    }
}