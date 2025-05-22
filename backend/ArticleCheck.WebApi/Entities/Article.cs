namespace ArticleCheck.WebApi.Entities
{
    public class Article
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

        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
    }
}
