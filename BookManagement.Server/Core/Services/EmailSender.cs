using System.Net;
using System.Net.Mail;

namespace BookManagement.Server.Core.Services;
public class EmailSender
{
    private readonly IConfiguration _configuration;

    public EmailSender(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    public Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        string Host =  _configuration.GetValue<string>("Email:Host") ??"smtp.gmail.com";
        int Port = _configuration.GetValue<int>("Email:Port");
        string UserName = _configuration["Email:Username"] ?? "";
        string Password = _configuration["Email:Password"] ?? "";

        SmtpClient client = new SmtpClient
        {
            Port = Port,
            Host = Host,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(UserName, Password)
        };

        var mailMessage = new MailMessage() {
            From = new MailAddress(UserName),
            To = { new MailAddress(email) },
            Subject = subject,
            IsBodyHtml = true,
            Body = htmlMessage
        };

        return client.SendMailAsync(mailMessage);
    }
}