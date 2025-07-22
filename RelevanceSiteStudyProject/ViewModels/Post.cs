using System.ComponentModel.DataAnnotations;

namespace RelevanceSiteStudyProject.ViewModels
{
    public class Post
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required.")]
        [StringLength(100, ErrorMessage = "Title cannot be longer than 100 characters.")]
        public string Title { get; set; } = string.Empty;
        [Required(ErrorMessage = "Content is required.")]
        [StringLength(500, ErrorMessage = "Content cannot be longer than 500 characters.")]
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Navigation properties
        public string UserId { get; set; } = string.Empty;
        public User User { get; set; } = null!;
        // Foreign key for the category
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }


}
