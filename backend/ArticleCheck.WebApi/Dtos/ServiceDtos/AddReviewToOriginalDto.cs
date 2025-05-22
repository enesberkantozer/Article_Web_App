namespace ArticleCheck.WebApi.Dtos.ServiceDtos
{
    public class AddReviewToOriginalDto
    {
        public List<RatingDto> Ratinglist {  get; set; } = new List<RatingDto>();
        public string Filepath { get; set; }
        public string Tempfilename { get; set; }
    }
}
