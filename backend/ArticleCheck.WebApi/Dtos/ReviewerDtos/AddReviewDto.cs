using ArticleCheck.WebApi.Entities;

namespace ArticleCheck.WebApi.Dtos.ReviewerDtos
{
    public class AddReviewDto
    {
        public float RatingValue { get; set; }
        public string Title { get; set; }

        public string Comment { get; set; }

        public int ArticleId { get; set; }
        public int ReviewerId { get; set; }
    }
}
