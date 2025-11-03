using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MimeKit;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using ASP.NETCore5.Models;
using ASP.NETCore5.Settings;


namespace ASP.NETCore5.Services
{
    public class MailService : IMailService
    {
        private readonly MailSettings _settings;

        public MailService(IOptions<MailSettings> mailSettings)
        {
            _settings = mailSettings.Value;
        }

        public async Task SendEmailAsync(MailRequest mailRequest)
        {
            var email = new MimeMessage();
            email.Sender = MailboxAddress.Parse(_settings.Mail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToEmail));
            email.Subject = mailRequest.Subject;

            //var builder = new BodyBuilder();
            //builder.HtmlBody = mailRequest.Body;
            email.Body = new TextPart("html") { Text = mailRequest.Body };

            using var smtp = new SmtpClient();
            smtp.Connect(_settings.Host, _settings.Port, SecureSocketOptions.SslOnConnect);
        
            smtp.Authenticate(_settings.Mail, _settings.Password);
            await smtp.SendAsync(email);
            smtp.Disconnect(true);
        }
    }
}
