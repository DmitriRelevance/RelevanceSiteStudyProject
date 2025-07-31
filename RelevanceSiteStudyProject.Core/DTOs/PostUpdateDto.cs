using System.ComponentModel.DataAnnotations;

namespace RelevanceSiteStudyProject.Core.DTOs
{
    public class PostUpdateDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Invalid post ID, must be greater than 0.")]
        public int Id { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Title { get; set; } = string.Empty;
        [Required]
        [StringLength(100, MinimumLength = 3)]
        public string Content { get; set; } = string.Empty;
    }
}
