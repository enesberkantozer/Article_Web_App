using ArticleCheck.WebApi.Context;
using ArticleCheck.WebApi.Dtos.AdminDtos;
using ArticleCheck.WebApi.Dtos.ArticleDtos;
using ArticleCheck.WebApi.Dtos.ReviewerDtos;
using ArticleCheck.WebApi.Entities;
using ArticleCheck.WebApi.Libraries;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ArticleCheck.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly ServiceAddReview _serviceReview;

        public AdminController(ApiContext context, ServiceAddReview serviceReview)
        {
            _context = context;
            _serviceReview = serviceReview;
        }

        [HttpGet("GetAll")]
        public async Task<IActionResult> GetArticles()
        {
            List<Article>? list = await _context.Articles.Include(a => a.Messages).Include(a => a.Ratings).ToListAsync();
            Log log = new Log() { CreatedAt = DateTime.Now , LogMessage= "Bütün makaleler çekildi" , Type="Başarılı"};
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            List<GetArticleDto> getList= new List<GetArticleDto>();
            foreach (var item in list)
            {
                GetArticleDto a = new GetArticleDto()
                {
                    Id = item.Id,
                    UploadedTime = DateTime.Now,
                    UpdateTime = DateTime.Now,
                    Title = item.Title,
                    Status = item.Status,
                    AuthorMail = item.AuthorMail,
                    FilePath = item.FilePath,
                    isChangeable = item.isChangeable,
                    TrackingCode = item.TrackingCode
                };
                getList.Add(a);
            }
            return Ok(getList);
        }


        [HttpGet("GetCheckProcessArticles")]
        public async Task<IActionResult> GetCheckProcessArticles()
        {
            List<Article>? articles = await _context.Articles.Where(
                a => a.Status != "Waiting Admin Approve" && a.Status != "Waiting Reviewers Comment" && a.Status != "Completed").ToListAsync();
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = "Kontrol süreci bekleyen makaleler çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();

            return Ok(articles);
        }

        [HttpGet("GetWaitingReviewersComment")]
        public async Task<IActionResult> GetWaitingReviewersComment()
        {
            List<Article>? articles = await _context.Articles.Where(a => a.Status == "Waiting Reviewers Comment").ToListAsync();
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = "Hakemler tarafından yorum bekleyen makaleler çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();

            return Ok(articles);
        }

        [HttpGet("GetWaitingAdminApprove")]
        public async Task<IActionResult> GetWaitingAdminApprove()
        {
            List<Article>? articles = await _context.Articles.Where(a => a.Status == "Waiting Admin Approve").ToListAsync();
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = "Yazara yönlendirme onayı bekleyen makaleler çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();

            return Ok(articles);
        }

        [HttpGet("GetCompletedArticles")]
        public async Task<IActionResult> GetCompletedArticles()
        {
            List<Article>? articles = await _context.Articles.Where(a => a.Status == "Completed").ToListAsync();
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = "Tamamlanmış makaleler çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();

            return Ok(articles);
        }

        [HttpGet("GetLogs")]
        public async Task<IActionResult> GetLogs()
        {
            List<Log>? logs = await _context.Logs.OrderByDescending(l => l.CreatedAt).ToListAsync();
            return Ok(logs);
        }

        [HttpGet("GetWithId/{id}")]
        public async Task<IActionResult> GetArticle(int id)
        {
            Article? article = await _context.Articles.Include(a=>a.Reviewers).Where(a=> a.Id==id).FirstOrDefaultAsync();
            if (article == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale çekilemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound();
            }
            GetArticleDto getArticle = new GetArticleDto()
            {
                Id = article.Id,
                AuthorMail = article.AuthorMail,
                FilePath = article.FilePath,
                isChangeable = article.isChangeable,
                Status = article.Status,
                Title = article.Title,
                TrackingCode = article.TrackingCode,
                UpdateTime = article.UpdateTime,
                UploadedTime = article.UploadedTime
            };
            foreach(var reviewer in article.Reviewers)
            {
                GetReviewerFromAdminDto dto = new GetReviewerFromAdminDto()
                {
                    Id = reviewer.Id,
                    Mail = reviewer.Mail,
                    Name = reviewer.Name
                };
                getArticle.Reviewers.Add(dto);
            }
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok(getArticle);
        }

        [HttpPut("update")]
        public async Task<IActionResult> UpdateArticle(UpdateArticleDto dto)
        {
            Article? article=await _context.Articles.FindAsync(dto.Id);
            if (article == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{dto.Id} id numaralı makale güncellenemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound("Article not found");
            }
            article.Status= dto.Status;
            article.isChangeable = dto.isChangeable;
            _context.Articles.Update(article);

            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{dto.Id} id numaralı makale güncellendi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            List<Article>? list = await _context.Articles.Where(
                a => a.Status != "Waiting Admin Approve" && a.Status != "Waiting Reviewers Comment" && a.Status != "Completed").ToListAsync();
            return Ok(list);
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            Article? article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale silinemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound("Article not found");
            }
            _context.Articles.Remove(article);
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{id} id numaralı makale silindi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            List<Article>? list = await _context.Articles.ToListAsync();
            return Ok(list);
        }

        [HttpPut("AddReviewsToOriginal/{articleId}")]
        public async Task<IActionResult> AddReviewsToOriginal(int articleId)
        {
            Article? article=await _context.Articles.FindAsync(articleId);
            if (article==null)
            {
                Log logWarning = new Log()
                {
                    CreatedAt = DateTime.Now,
                    LogMessage = $"{articleId} id nolu makale süreci tamamlanamadı",
                    Type = "Uyarı"
                };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound($"{articleId} article id not found");
            }
            else
            {
                if (await _serviceReview.Run("original", articleId))
                {
                    article.Status = "Completed";
                    _context.Articles.Update(article);

                    Log log = new Log()
                    {
                        CreatedAt = DateTime.Now,
                        LogMessage = $"{articleId} id nolu makale süreci başarıyla tamamladı",
                        Type = "Başaralı"
                    };
                    await _context.Logs.AddAsync(log);

                    await _context.SaveChangesAsync();
                    return Ok();
                }
                else
                {
                    Log logError = new Log()
                    {
                        CreatedAt = DateTime.Now,
                        LogMessage = $"{articleId} id nolu makale süreci tamamlanamadı",
                        Type = "Hata"
                    };
                    await _context.Logs.AddAsync(logError);
                    await _context.SaveChangesAsync();
                    return BadRequest($"{articleId} article id reviews cannot write pdf file");
                }
            }
        }
    }
}
