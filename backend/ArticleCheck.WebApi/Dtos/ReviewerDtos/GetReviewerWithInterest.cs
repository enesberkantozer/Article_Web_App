using ArticleCheck.WebApi.Dtos.ArticleDtos;
using ArticleCheck.WebApi.Entities;

namespace ArticleCheck.WebApi.Dtos.ReviewerDtos
{
    public class GetReviewerWithInterest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }

        public List<GetArticleDto> Articles { get; set; }=new List<GetArticleDto>();
    }
}
