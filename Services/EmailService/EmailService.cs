using MailKit.Net.Smtp;
using MimeKit;
namespace WebApi.Services.EmailService

{
    public class EmailService
    {
        public readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<string> SendEmailAsync(string toEmail, string subject, string message)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration.GetSection("EmailConfig:Email").Value));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message
            };

            using var smpt = new SmtpClient();
            
            try
            {
                smpt.Connect("smtp.gmail.com", 587, MailKit.Security.SecureSocketOptions.StartTls);
                smpt.Authenticate(_configuration.GetSection("EmailConfig:Email").Value, _configuration.GetSection("EmailConfig:Password").Value);
                smpt.Send(email);
            }
            catch(Exception ex)
            {
                return ex.Message;
            }
            smpt.Disconnect(true);
            return "Message sent";
        }
    }
}
