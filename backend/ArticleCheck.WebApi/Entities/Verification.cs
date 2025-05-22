namespace ArticleCheck.WebApi.Entities
{
    public class Verification
    {
        public int Id { get; set; }
        public string EMail  { get; set; }
        public int VerificationCode { get; set; }
        public DateTime ExpirationTime { get; set; }
    }
}
