using ArticleCheck.WebApi.Context;
using ArticleCheck.WebApi.Dtos.ArticleDtos;
using ArticleCheck.WebApi.Dtos.InterestDtos;
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
    public class ReviewersController : ControllerBase
    {
        private readonly ApiContext _context;
        private readonly ServiceAddReview _serviceReview;

        public ReviewersController(ApiContext context, ServiceAddReview serviceReview)
        {
            _context = context;
            _serviceReview = serviceReview;
        }

        [HttpGet("isReviewer/{mail}")]
        public async Task<IActionResult> IsReviewer(string mail)
        {
            Reviewer? reviewer=_context.Reviewers.Where(r => r.Mail == mail).FirstOrDefault();
            if (reviewer == null)
            {
                Log logFalse = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{mail} hakem olmadığı doğrulandı", Type = "Başarılı" };
                await _context.Logs.AddAsync(logFalse);
                await _context.SaveChangesAsync();
                return Ok(false);
            }
            else
            {
                Log logTrue = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{mail} hakem olduğu doğrulandı", Type = "Başarılı" };
                await _context.Logs.AddAsync(logTrue);
                await _context.SaveChangesAsync();
                return Ok(true);
            }
        }

        [HttpGet("GetAllInterests")]
        public async Task<IActionResult> GetAllWithInterest()
        {
            List<Interest>? interests = await _context.Interests.ToListAsync();
            if(interests==null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"İlgi alanları çekilemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound("Interest not found");
            }
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"İlgi alanları çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok(interests);
        }

        [HttpGet("GetWithInterest/{articleId}/{interestId}")]
        public async Task<IActionResult> GetFromInterest(int interestId,int articleId)
        {
            List<Reviewer>? reviewers = await _context.Reviewers
                .Where(r => r.Interests.Any(i => i.Id == interestId))
                .Include(r=>r.Articles)
                .ToListAsync();

            List<GetReviewerWithInterest> getReviewers = new List<GetReviewerWithInterest> ();
            for(int i = 0; i < reviewers.Count; i++)
            {
                if (reviewers[i].Articles == null)
                    continue;

                if (!reviewers[i].Articles.Any(a => a.Id == articleId))
                {
                    GetReviewerWithInterest a = new GetReviewerWithInterest();
                    a.Id = reviewers[i].Id;
                    a.Name = reviewers[i].Name;
                    a.Mail = reviewers[i].Mail;
                    getReviewers.Add(a);
                }
            }
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{interestId} id no ilgi alanına sahip ve daha önce {articleId} id no makaleye yorum yapmamış hakemler çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok(getReviewers);
        }

        [HttpGet("Get/{mail}")]
        public async Task<IActionResult> GetMyArticles(string mail)
        {
            Reviewer? reviewer = await _context.Reviewers
                .Include(r => r.Articles)
                .Include(r => r.Interests)
                .FirstOrDefaultAsync(r => r.Mail == mail);
            if (reviewer == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{mail} hakem çekilemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound("Reviewer not found");
            }

            GetReviewerDto getReviewer=new GetReviewerDto();
            getReviewer.Id = reviewer.Id;
            getReviewer.Name = reviewer.Name;
            getReviewer.Mail = reviewer.Mail;
            foreach (var rate in reviewer.Interests)
            {
                GetInterestFromReviewerDto interest=new GetInterestFromReviewerDto();
                interest.Id = rate.Id;
                interest.Topic = rate.Topic;
                getReviewer.Interests.Add(interest);
            }
            foreach (var item in reviewer.Articles)
            {
                if(await _context.Ratings.Where(r=>r.Article.Id == item.Id && r.Reviewer.Id==reviewer.Id).AnyAsync())
                {
                    GetArticleFromReviewerDto article = new GetArticleFromReviewerDto();
                    article.Id = item.Id;
                    article.Title = item.Title;
                    article.FilePath = item.FilePath;
                    getReviewer.OldArticles.Add(article);
                }
                else
                {
                    GetArticleFromReviewerDto article = new GetArticleFromReviewerDto();
                    article.Id = item.Id;
                    article.Title = item.Title;
                    article.FilePath = item.FilePath;
                    getReviewer.Articles.Add(article);
                }
            }
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{mail} hakem çekildi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok(getReviewer);
        }

        [HttpPut("AddReviewersToArticle")]
        public async Task<IActionResult> AddReviewerToArticle(AddReviewerToArticle dto)
        {
            List<Reviewer> reviewers = await _context.Reviewers.Where(r=> dto.Reviewers.Contains(r.Id)).ToListAsync();
            Article? article = await _context.Articles.Include(a=>a.Reviewers).Where(a=>a.Id==dto.ArticleId).FirstOrDefaultAsync();
            if (article == null)
            {
                Log logWarning = new Log() { CreatedAt = DateTime.Now, LogMessage = $"Seçili hakemler {dto.ArticleId} id nolu makaleye eklenemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound("Article or Reviewer not found");
            }
            if (reviewers.Count() == 0)
            {
                article.Status = "Checking";
                Log log1 = new Log()
                {
                    CreatedAt = DateTime.Now,
                    LogMessage = $"{article.Id} id nolu makalenin durumu {article.Status} olarak güncellendi",
                    Type = "Başarılı"
                };
                await _context.Logs.AddAsync(log1);
            }
            else
            {
                article.Status = "Waiting Reviewers Comment";
                Log log1 = new Log()
                {
                    CreatedAt = DateTime.Now,
                    LogMessage = $"{article.Id} id nolu makalenin durumu {article.Status} olarak güncellendi",
                    Type = "Başarılı"
                };
                await _context.Logs.AddAsync(log1);
            }
            List<Reviewer>? allReviewers = await _context.Reviewers.Include(r => r.Articles).Where(r => r.Articles.Any(a => a.Id == dto.ArticleId)).ToListAsync();
            foreach (Reviewer reviewer in allReviewers)
            {
                reviewer.Articles.Remove(article);
            }
            _context.Reviewers.UpdateRange(allReviewers);
            List<Rating>? allRatings= await _context.Ratings.Include(r=>r.Article).Where(r=>r.Article.Id==dto.ArticleId).ToListAsync();
            _context.Ratings.RemoveRange(allRatings);
            article.Reviewers.Clear();
            foreach (Reviewer item in reviewers)
            {
                if(item.Articles==null)
                    item.Articles= new List<Article>();
                if(article.Reviewers==null)
                    article.Reviewers= new List<Reviewer>();

                if (!item.Articles.Any(a => a.Id == article.Id))
                {
                    item.Articles.Add(article);
                    _context.Reviewers.Update(item);
                }
                if (!article.Reviewers.Any(r=>r.Id==item.Id))
                    article.Reviewers.Add(item);
            }
            _context.Articles.Update(article);
            Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"Seçili hakemler {dto.ArticleId} id nolu makaleye eklendi", Type = "Başarılı" };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();
            return Ok("Success");
        }

        [HttpPost("AddReview")]
        public async Task<IActionResult> AddReview(AddReviewDto dto)
        {
            Article? article = await _context.Articles.FindAsync(dto.ArticleId);
            Reviewer? reviewer = await _context.Reviewers.FindAsync(dto.ReviewerId);

            if (article == null || reviewer == null)
            {
                Log logWarning = new Log()
                {
                    CreatedAt = DateTime.Now,
                    LogMessage = $"{dto.ArticleId} veya {dto.ReviewerId} bulunamadığı için rating tablosuna ekleme yapılamadı",
                    Type = "Uyarı"
                };
                await _context.Logs.AddAsync(logWarning);
                await _context.SaveChangesAsync();
                return NotFound($"{dto.ArticleId} or {dto.ReviewerId} not found");
            }

            Rating rating = new Rating()
            {
                Article = article,
                Reviewer = reviewer,
                Comment = dto.Comment,
                Title = dto.Title,
                RatingValue = dto.RatingValue,
                RatingDate = DateTime.Now
            };
            await _context.Ratings.AddAsync(rating);

            Log log = new Log()
            {
                CreatedAt = DateTime.Now,
                LogMessage = $"{dto.ArticleId} id nolu makaleye {dto.ReviewerId} id nolu hakem yorum yaptı",
                Type = "Başarılı"
            };
            await _context.Logs.AddAsync(log);
            await _context.SaveChangesAsync();

            int articleTotalReviewerCount = await _context.Reviewers.CountAsync(r=> r.Articles.Any(a=>a.Id==dto.ArticleId));
            int totalRating = await _context.Ratings.CountAsync(r => r.Article.Id == dto.ArticleId);

            if(articleTotalReviewerCount == totalRating)
            {
                article.Status = "Waiting Admin Approve";
                _context.Articles.Update(article);
                await _context.SaveChangesAsync();
                if (await _serviceReview.Run("anonym", article.Id))
                {
                    return Ok();
                }
                else
                {
                    return BadRequest();
                }
            }

            return Ok();
        }
    }
}
