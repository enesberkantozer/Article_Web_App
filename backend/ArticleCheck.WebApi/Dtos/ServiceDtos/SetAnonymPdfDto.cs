namespace ArticleCheck.WebApi.Dtos.ServiceDtos
{
    public class SetAnonymPdfDto
    {
        public string PdfPath { get; set; }
        public string PdfName { get; set; }
        public bool HideName { get; set; }
        public bool HideCompany { get; set; }
        public bool HideMailPhone { get; set; }
    }
}
