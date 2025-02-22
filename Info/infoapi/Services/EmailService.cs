using infoapi.Interfaces;
using System.Net;
using System.Net.Mail;

namespace infoapi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendOtpEmail(string toemail, string otp)
        {

            var smtpClient = new SmtpClient(_configuration["Email:SmtpServer"])
            {
                Port = int.Parse(_configuration["Email:Port"]),
                Credentials = new NetworkCredential(_configuration["Email:Username"], _configuration["Email:Password"]),
                EnableSsl = true,
                UseDefaultCredentials = false
            };

            var mailMessage = new MailMessage()
            {
                From = new MailAddress(_configuration["Email:UserName"]),
                Subject = "Password Reset OTP",
                Body = $"Your OTP for password reset is : {otp}",
                IsBodyHtml = false
            };
            mailMessage.To.Add(toemail);

            await smtpClient.SendMailAsync(mailMessage);
        }
    }
}
