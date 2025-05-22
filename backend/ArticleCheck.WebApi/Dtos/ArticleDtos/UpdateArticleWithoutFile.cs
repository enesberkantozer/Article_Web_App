using ArticleCheck.WebApi.Entities;

namespace ArticleCheck.WebApi.Dtos.ArticleDtos
{
    public class UpdateArticleWithoutFile
    {
        public string Title { get; set; }
        public Guid TrackingCode { get; set; }
        public string Status { get; set; }
        public bool isChangeable { get; set; }
        public DateTime UpdateTime { get; set; }
    }
}
