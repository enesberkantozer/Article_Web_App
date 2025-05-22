using ArticleCheck.WebApi.Context;
using ArticleCheck.WebApi.Dtos.MailDtos;
using ArticleCheck.WebApi.Entities;
using ArticleCheck.WebApi.Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArticleCheck.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MailsController : ControllerBase
    {
        private readonly ApiContext _context;

        public MailsController(ApiContext context)
        {
            _context = context;
        }

        [HttpPost("sendVerification/{email}")]
        public async Task<IActionResult> sendVerification(string email)
        {
            List<Verification>? verifications = await _context.Verifications.Where(v => v.EMail == email).ToListAsync();
            if(verifications !=null)
            {
                if (verifications.Count > 0)
                {
                    _context.Verifications.RemoveRange(verifications);
                    await _context.SaveChangesAsync();
                }
            }
            Verification verification = new Verification();
            verification.EMail = email;
            verification.VerificationCode = new Random().Next(100000, 999999);

            bool isSend=await MailSender.SendMail(email, "Verification Code", $"Your verification code is {verification.VerificationCode}");
            if(!isSend)
            {
                Log logError = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{verification.EMail} doğrulama kodu gönderilemedi", Type = "Hata" };
                await _context.Logs.AddAsync(logError);
                await _context.SaveChangesAsync();
                return BadRequest("Mail not sent");
            }

            verification.ExpirationTime = DateTime.Now.AddMinutes(3);
            await _context.Verifications.AddAsync(verification);
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{verification.EMail} doğrulama kodu gönderildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok("Success");
        }

        [HttpGet("check/{email}")]
        public async Task<IActionResult> CheckCode(string email)
        {
            Verification? verification = await _context.Verifications.FirstOrDefaultAsync(v => v.EMail == email && v.ExpirationTime>DateTime.Now);
            if (verification == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{email} sistemde kayıtlı doğrulama kodu yok", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return BadRequest("Invalid or expired code");
            }
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{email} doğrulama kodu bulundu ve kontrolden geçti", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok(verification.VerificationCode);
        }

        [HttpPost("sendMail")]
        public async Task<IActionResult> SendMail(CreateMailSenderDto mailSenderDto)
        {
            if(await MailSender.SendMail(mailSenderDto.Email, mailSenderDto.Subject, mailSenderDto.Body))
            {
                Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{mailSenderDto.Email} mail gönderildi", Type = "Başarılı" };
                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();
                return Ok("Success");
            }
            else
            {
                Log logError = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{mailSenderDto.Email} mail gönderilemedi", Type = "Hata" };
                await _context.Logs.AddAsync(logError);
                await _context.SaveChangesAsync();
                return BadRequest("Mail not sent");
            }
        }

        [HttpDelete("deleteVerification/{email}")]
        public async Task<IActionResult> DeleteVerification(string email)
        {
            List<Verification>? verification = await _context.Verifications.Where(v => v.EMail == email).ToListAsync();
            if (verification == null)
            {
                Log logError = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{email} süresi geçmiş doğrulama kodları silinemedi", Type = "Hata" };
                await _context.Logs.AddAsync(logError);
                await _context.SaveChangesAsync();
                return BadRequest("Email not found");
            }
            _context.Verifications.RemoveRange(verification);
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{email} süresi geçmiş doğrulama kodları silindi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok("Success");
        }
    }
}
