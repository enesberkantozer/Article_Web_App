namespace ArticleCheck.WebApi.Entities
{
    public class Interest
    {
        public int Id { get; set; }
        public string Topic { get; set; }

        public ICollection<Reviewer> Reviewers { get; set; } = new List<Reviewer>();
    }
}
