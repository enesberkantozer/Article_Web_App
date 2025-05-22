using ArticleCheck.WebApi.Dtos.ReviewerDtos;

namespace ArticleCheck.WebApi.Dtos.ArticleDtos
{
    public class GetArticleDto
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string AuthorMail { get; set; }
        public Guid TrackingCode { get; set; }
        public string FilePath { get; set; }
        public string Status { get; set; }
        public bool isChangeable { get; set; }
        public DateTime UploadedTime { get; set; }
        public DateTime UpdateTime { get; set; }

        public List<GetReviewerFromAdminDto> Reviewers { get; set; }=new List<GetReviewerFromAdminDto>();
    }
}
