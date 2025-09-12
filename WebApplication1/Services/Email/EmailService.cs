

using DotNetEnv;
using System.Net;
using System.Net.Mail;

namespace Food_Ordering.Services.Email
{
    public class EmailService : IEmailService
    {
        public async Task SendEmailAsync(string email, string subject, string body)
        {
            Env.Load();

            var host = Env.GetString("SMTP_HOST");
            var port = Env.GetInt("SMTP_PORT");
            var enableSsl = Env.GetBool("SMTP_ENABLE_SSL");
            var userName = Env.GetString("SMTP_USERNAME");
            var authPassword = Env.GetString("SMTP_PASSWORD");
            var senderEmail = Env.GetString("SMTP_FROM_EMAIL");
            var senderName = Env.GetString("SMTP_FROM_NAME");

            var mailMessage = new MailMessage
            {
                From = new MailAddress(senderEmail, senderName),
                Subject = subject,
                Body = body,
                IsBodyHtml = true,
            };

            mailMessage.To.Add(email);

            using (var client = new SmtpClient(host, port)) { 
                client.EnableSsl = enableSsl;
                client.Credentials = new NetworkCredential(userName, authPassword);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;

                await client.SendMailAsync(mailMessage);
            }
        }
    }
}
