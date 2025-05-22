namespace ArticleCheck.WebApi.Dtos.ArticleDtos
{
    public class UpdateArticleWithFileDto
    {
        public Guid TrackingCode { get; set; }
        public IFormFile File { get; set; }
    }
}
