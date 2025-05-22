using ArticleCheck.WebApi.Context;
using ArticleCheck.WebApi.Dtos.ArticleDtos;
using ArticleCheck.WebApi.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IoFile=System.IO.File;

namespace ArticleCheck.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ArticlesController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _env;

        public ArticlesController(ApiContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        [HttpGet("{trackingCode}")]
        public async Task<IActionResult> GetArticle(Guid trackingCode)
        {
            Article? article = await _context.Articles
                .Include(a => a.Messages)
                .Include(a => a.Ratings)
                .FirstOrDefaultAsync(a => a.TrackingCode == trackingCode);
            if (article == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{trackingCode} takip nolu makale bulunamadı", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound($"Article with id {trackingCode} not found");
            }
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{trackingCode} takip nolu makale çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            GetArticleDto getArticle = new GetArticleDto()
            {
                TrackingCode = article.TrackingCode,
                Id = article.Id,
                AuthorMail = article.AuthorMail,
                FilePath = article.FilePath,
                isChangeable = article.isChangeable,
                Status = article.Status,
                Title = article.Title,
                UpdateTime = article.UpdateTime,
                UploadedTime = article.UploadedTime
            };
            return Ok(getArticle);
        }

        [HttpPost("upload")]
        public async Task<IActionResult> AddArticle([FromForm] CreateArticleDto model)
        {
            if (model.File == null || model.File.Length == 0)
            {
                Log logError = new Log() { CreatedAt = DateTime.Now, LogMessage = "Makale dosyası yükleme hatası", Type = "Hata" };
                await _context.Logs.AddAsync(logError);
                await _context.SaveChangesAsync();
                return BadRequest("Dosya yüklenmedi.");
            }
            Article article = new Article();
            article.AuthorMail = model.Email;
            article.Title = model.Title;
            article.TrackingCode = Guid.NewGuid();
            article.Status = "Checking";
            article.isChangeable = false;
            article.UploadedTime = DateTime.Now;
            article.UpdateTime = DateTime.Now;

            string absDirectory = Path.Combine(_env.WebRootPath, "uploads", @$"{article.TrackingCode}");
            string absPathOriginal = Path.Combine(absDirectory, "original" + (Path.GetExtension(model.File.FileName)));
            string absPathAnonym= Path.Combine(absDirectory, "anonym"+(Path.GetExtension(model.File.FileName)));
            string clientDirPath = @$"uploads/{article.TrackingCode}/";
            if(!Directory.Exists(absDirectory))
            {
                Directory.CreateDirectory(absDirectory);
            }
            try
            {
                using(var fileStream = new FileStream(absPathOriginal, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }
                using (var fileStream = new FileStream(absPathAnonym, FileMode.Create))
                {
                    await model.File.CopyToAsync(fileStream);
                }
                article.FilePath = clientDirPath;
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
            await _context.Articles.AddAsync(article);
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{article.TrackingCode} takip nolu makale yüklendi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok(article.TrackingCode);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            Article? article = await _context.Articles.FindAsync(id);
            if (article==null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale silinemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound($"Delete failed. {id} article not found");
            }
            _context.Articles.Remove(article);
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale silindi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok("Article deleted successfully");
        }

        [HttpPut]
        public async Task<IActionResult> UpdateArticle(UpdateArticleWithoutFile article)
        {
            Article? articleToUpdate = await _context.Articles.Where(a => a.TrackingCode == article.TrackingCode).FirstOrDefaultAsync();
            if (articleToUpdate == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{article.TrackingCode} takip nolu makale güncellenemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound($"Update failed. {article.TrackingCode} article not found");
            }
            articleToUpdate.Title = article.Title;
            articleToUpdate.Status = article.Status;
            articleToUpdate.UpdateTime = DateTime.Now;
            _context.Articles.Update(articleToUpdate);
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{article.TrackingCode} takip nolu makale güncellendi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok(articleToUpdate);
        }

        [HttpPut("WithFile")]
        public async Task<IActionResult> UpdateArticleWithFile(UpdateArticleWithFileDto article)
        {
            Article? articleToUpdate = await _context.Articles.Where(a => a.TrackingCode == article.TrackingCode).FirstOrDefaultAsync();
            if (articleToUpdate == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{article.TrackingCode} takip nolu makale dosyası güncellenemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound($"Update failed. {article.TrackingCode} article not found");
            }
            if(article.File==null || article.File.Length == 0)
            {
                Log logError = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{article.TrackingCode} takip nolu makale dosyası güncellenemedi", Type = "Hata" };
                await _context.Logs.AddAsync(logError);
                await _context.SaveChangesAsync();
                return BadRequest("Dosya yüklenmedi.");
            }
            string absDirectory = Path.Combine(_env.WebRootPath, "uploads", @$"{article.TrackingCode}");
            string absPathOriginal = Path.Combine(absDirectory, "original" + (Path.GetExtension(article.File.FileName)));
            string absPathAnonym = Path.Combine(absDirectory, "anonym" + (Path.GetExtension(article.File.FileName)));
            string prevPath = Path.Combine(absDirectory, "prev"+(Path.GetExtension(absPathOriginal)));
            if (!Directory.Exists(absDirectory))
            {
                Directory.CreateDirectory(absDirectory);
            }
            try
            {
                bool isCopied = false;
                IoFile.Move(absPathOriginal,prevPath,true);
                using (var fileStream = new FileStream(absPathOriginal, FileMode.Create))
                {
                    await article.File.CopyToAsync(fileStream);
                    IoFile.Copy(absPathOriginal,absPathAnonym,true);
                    isCopied = true;
                }
                if (isCopied)
                {
                    IoFile.Delete(prevPath);
                }
            }
            catch (Exception e)
            {
                if(IoFile.Exists(prevPath))
                    IoFile.Move(prevPath,absPathOriginal,true);
                return BadRequest(e.Message);
            }
            articleToUpdate.Status = "Checking";
            articleToUpdate.UpdateTime = DateTime.Now;
            _context.Articles.Update(articleToUpdate);
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{article.TrackingCode} takip nolu makale dosyası güncellendi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok("Article updated successfully");
        }

    }
}
