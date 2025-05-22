namespace ArticleCheck.WebApi.Dtos.ReviewerDtos
{
    public class AddReviewerToArticle
    {
        public List<int> Reviewers { get; set; }
        public int ArticleId { get; set; }
    }
}
