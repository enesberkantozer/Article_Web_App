namespace ArticleCheck.WebApi.Entities
{
    public class Rating
    {
        public int Id { get; set; }
        public float RatingValue { get; set; }
        public string Title { get; set; }
        public string Comment { get; set; }
        public DateTime RatingDate { get; set; }
        public Article Article { get; set; }
        public Reviewer Reviewer { get; set; }
    }
}
