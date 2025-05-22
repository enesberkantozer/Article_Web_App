using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ArticleCheck.WebApi.Libraries
{
    public class MailSender
    {

        public static async Task<bool> SendMail(string to, string subject, string body)
        {
            try
            {
                SmtpClient smtpClient = new SmtpClient("smtp.gmail.com")
                {
                    Port = 587,
                    Credentials = new NetworkCredential("developerenesberkant@gmail.com", "wtkloeskqsvmaguh"),
                    EnableSsl = true,
                };

                MailMessage mailMessage = new MailMessage
                {
                    From = new MailAddress("developerenesberkant@gmail.com"),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = false,
                };

                mailMessage.To.Add(to);

                await smtpClient.SendMailAsync(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}
