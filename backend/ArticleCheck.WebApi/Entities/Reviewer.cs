namespace ArticleCheck.WebApi.Entities
{
    public class Reviewer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Mail { get; set; }

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Interest> Interests { get; set; } = new List<Interest>();
        public ICollection<Article> Articles { get; set; } = new List<Article>();
    }
}
