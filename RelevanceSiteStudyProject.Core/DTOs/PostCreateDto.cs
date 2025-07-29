namespace RelevanceSiteStudyProject.Core.DTOs
{
    public class PostCreateDto
    {
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}
