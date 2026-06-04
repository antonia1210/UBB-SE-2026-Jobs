using Microsoft.AspNetCore.Http;

namespace UBB_SE_2026_Jobs.Library.DTOs
{
    public class VideoUploadDto
    {
        public IFormFile File { get; set; }
    }
}
