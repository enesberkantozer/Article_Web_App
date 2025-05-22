using ArticleCheck.WebApi.Context;
using ArticleCheck.WebApi.Dtos.ServiceDtos;
using ArticleCheck.WebApi.Entities;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace ArticleCheck.WebApi.Libraries
{
    public class ServiceAddReview
    {
        private readonly HttpClient _httpClient;
        private readonly ApiContext _context;
        private readonly IWebHostEnvironment _env;

        public ServiceAddReview(HttpClient httpClient, ApiContext context, IWebHostEnvironment env)
        {
            _httpClient = httpClient;
            _context = context;
            _env = env;
        }

        public async Task<bool> Run(string type, int articleId)
        {
            List<Rating>? ratings = await _context.Ratings.Include(r => r.Article).Include(r => r.Reviewer).Where(r => r.Article.Id == articleId).ToListAsync();

            if (ratings == null || ratings.Count == 0)
            {
                Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{articleId} id nolu {type} makaleye yorum eklenemedi", Type = "Uyarı" };
                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();
                return false;
            }

            AddReviewToOriginalDto addReviewToOriginalDto = new AddReviewToOriginalDto();
            addReviewToOriginalDto.Filepath = ratings[0].Article.FilePath + $"{type}.pdf";
            addReviewToOriginalDto.Tempfilename = ratings[0].Article.TrackingCode + ".pdf";
            foreach (Rating rating in ratings)
            {
                RatingDto ratingDto = new RatingDto()
                {
                    Comment = rating.Comment,
                    Title = rating.Title,
                    Ratingvalue = rating.RatingValue,
                    Time = rating.RatingDate.ToString("dd MMMM yyyy HH:mm")
                };
                addReviewToOriginalDto.Ratinglist.Add(ratingDto);
            }

            var content = new StringContent(JsonSerializer.Serialize(addReviewToOriginalDto), System.Text.Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync("http://127.0.0.1:5000/api/review/add", content);

            if (response.IsSuccessStatusCode)
            {
                var responseData = await response.Content.ReadAsStreamAsync();

                string savePath = Path.Combine(_env.WebRootPath, "uploads", ratings[0].Article.TrackingCode.ToString(), $"{type}.pdf");

                using (var fileStream = new FileStream(savePath, FileMode.Create, FileAccess.Write))
                {
                    await responseData.CopyToAsync(fileStream);
                }

                Log log = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{ratings[0].Article.Id} id'li {type} makaleye {ratings[0].Reviewer.Id} id'li  hakem tarafından yorum eklendi", Type = "Başarılı" };
                await _context.Logs.AddAsync(log);
                await _context.SaveChangesAsync();

                return true;
            }
            else
            {
                Log logError = new Log() { CreatedAt = DateTime.Now, LogMessage = $"{ratings[0].Article.Id} id'li {type} makaleye {ratings[0].Reviewer.Id} id'li hakem tarafından yorum eklenemedi", Type = "Hata" };
                await _context.Logs.AddAsync(logError);
                await _context.SaveChangesAsync();
                return false;
            }
        }
    }
}
