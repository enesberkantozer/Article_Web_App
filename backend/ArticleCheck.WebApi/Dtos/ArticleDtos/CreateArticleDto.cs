namespace ArticleCheck.WebApi.Dtos.ArticleDtos
{
    public class CreateArticleDto
    {
        public string Email { get; set; }
        public string Title { get; set; }
        public IFormFile File { get; set; }
    }
}
