namespace ArticleCheck.WebApi.Dtos.MailDtos
{
    public class CreateMailSenderDto
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
