namespace ArticleCheck.WebApi.Entities
{
    public class Log
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
        public string LogMessage { get; set; }
    }
}
