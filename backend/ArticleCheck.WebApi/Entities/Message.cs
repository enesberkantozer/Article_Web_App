namespace ArticleCheck.WebApi.Entities
{
    public class Message
    {
        public int Id { get; set; }
        public int ArticleId { get; set; }
        public string From { get; set; }
        public string MessageContent { get; set; }
        public DateTime MessageDate { get; set; }

        public Article Article { get; set; }
    }
}
