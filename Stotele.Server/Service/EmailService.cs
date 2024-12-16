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
        _senderEmail = sendGridSettings["SenderEmail"]
                       ?? throw new InvalidOperationException("SendGrid SenderEmail is not set.");
        _senderName = sendGridSettings["SenderName"]
                      ?? throw new InvalidOperationException("SendGrid SenderName is not set.");
    }

    public async Task<bool> SendEmailAsync(string recipientEmail, string subject, string message, string recipientName = "")
    {
        try
        {
            var client = new SendGridClient(_apiKey);
            var from = new EmailAddress(_senderEmail, _senderName);
            var to = new EmailAddress(recipientEmail, recipientName);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: message, htmlContent: message);

            var response = await client.SendEmailAsync(msg);
            Console.WriteLine($"SendGrid Response Status: {response.StatusCode}");

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Console.WriteLine($"Email successfully sent to {recipientEmail}");
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to send email to {recipientEmail}. Status Code: {response.StatusCode}");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occurred while sending email to {recipientEmail}: {ex.Message}");
            return false;
        }
    }
}
