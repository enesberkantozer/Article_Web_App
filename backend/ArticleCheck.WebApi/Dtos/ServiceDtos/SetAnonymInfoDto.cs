namespace ArticleCheck.WebApi.Dtos.ServiceDtos
{
    public class SetAnonymInfoDto
    {
        public int ArticleId { get; set; }
        public bool HideName { get; set; }
        public bool HideCompany { get; set; }
        public bool HideMailPhone { get; set; }
    }
}
