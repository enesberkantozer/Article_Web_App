namespace ArticleCheck.WebApi.Dtos.AdminDtos
{
    public class UpdateArticleDto
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public bool isChangeable { get; set; }
    }
}
