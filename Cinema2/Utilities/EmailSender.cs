using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Cinema2.Utilities
{
    public class EmailSender : IEmailSender
    {
        public Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,   //security
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("halaehab070@gmail.com", "wfqu mrfq igyv jjzn")
            };

            return client.SendMailAsync(
                new MailMessage(from: "halaehab070@gmail.com",
                to: email,
                subject,
                htmlMessage
                )
                {
                    IsBodyHtml = true
                }
                );
        }
    }
}
