using ArticleCheck.WebApi.Dtos.ArticleDtos;
using ArticleCheck.WebApi.Dtos.InterestDtos;

namespace ArticleCheck.WebApi.Dtos.ReviewerDtos
{
    public class GetReviewerDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }

        public List<GetInterestFromReviewerDto> Interests { get; set; } = new List<GetInterestFromReviewerDto>();
        public List<GetArticleFromReviewerDto> OldArticles { get; set; } = new List<GetArticleFromReviewerDto>();
        public List<GetArticleFromReviewerDto> Articles { get; set; } = new List<GetArticleFromReviewerDto>();

    }
}
