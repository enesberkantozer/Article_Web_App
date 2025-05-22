using Microsoft.AspNetCore.Http;

namespace ArticleCheck.WebApi.Context
{
    public class FileUploadRequest
    {
        public string Email { get; set; }
        public string Title { get; set; }
        public IFormFile File { get; set; }
    }
}
