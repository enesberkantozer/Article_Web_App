using ArticleCheck.WebApi.Context;
using ArticleCheck.WebApi.Dtos.ReviewerDtos;
using ArticleCheck.WebApi.Dtos.ServiceDtos;
using ArticleCheck.WebApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ArticleCheck.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _env;

        public ServicesController(HttpClient httpClient, ApiContext context, IWebHostEnvironment env)
        {
            _httpClient = httpClient;
            _context = context;
            _env = env;
        }

        [HttpGet("getTopics/{id}")]
        public async Task<IActionResult> GetTopics(int id)
        {
            Article? article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale anahtar kelimeleri çekilemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound();
            }

            var response = await _httpClient.GetAsync($"http://127.0.0.1:5000/api/nlp/getTopics?path={article.FilePath}&filename={article.Id}");

            if (!response.IsSuccessStatusCode)
            {
                Log logError = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale anahtar kelimeleri çekilemedi", Type = "Hata" };
                await _context.Logs.AddAsync(logError);
                await _context.SaveChangesAsync();
                return BadRequest("Microservices error");
            }
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale anahtar kelimeleri çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok(await response.Content.ReadAsStringAsync());
        }

        [HttpPut("setAnonymPdf")]
        public async Task<IActionResult> SetAnonymPdf(SetAnonymInfoDto info)
        {
            Article? article = await _context.Articles.FindAsync(info.ArticleId);

            if (article == null)
            {
                Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{info.ArticleId} id nolu makale anonimleştirilemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();
                return NotFound($"{info.ArticleId} article not found");
            }

            SetAnonymPdfDto sendPdf = new SetAnonymPdfDto()
            {
                PdfPath = article.FilePath + "original.pdf",
                PdfName = article.TrackingCode.ToString() + ".pdf",
                HideName = info.HideName,
                HideCompany = info.HideCompany,
                HideMailPhone = info.HideMailPhone
            };

            var content = new StringContent(JsonSerializer.Serialize(sendPdf), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("http://127.0.0.1:5000/api/nlp/anonym", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStreamAsync();

                string savePath = Path.Combine(_env.WebRootPath, "uploads", article.TrackingCode.ToString() , "anonym.pdf");

                using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    await responseData.CopyToAsync(fileStream);
                }

                Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{article.Id} id'li makale anonimleştirildi", Type = "Başarılı" };
                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();

                return Ok(article.TrackingCode);
            }
            else
            {
                Log logError = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{article.Id} id'li makale anonimleştirilemedi", Type = "Hata" };
                await _context.Logs.AddAsync(logError);
                await _context.SaveChangesAsync();
                return BadRequest("Microservices error");
            }
        }
    }
}
