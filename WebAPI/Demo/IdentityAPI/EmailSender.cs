using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace WebAPI.Example.Identity;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var host = _config["Email:Host"];
        var port = _config["Email:Port"];
        var username = _config["Email:Username"];
        var password = _config["Email:Password"];
        
        Console.WriteLine($"{host}, {port}, {username}, {password}");
        
        var client = new SmtpClient(_config["Email:Host"], Convert.ToInt32(_config["Email:Port"]))
        {
            Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"]),
            EnableSsl = true
        };

        var mail = new MailMessage(from: _config["Email:Username"],to: email, subject, htmlMessage)
        {
            IsBodyHtml = true
        };
        
        await client.SendMailAsync(mail);
    }
}