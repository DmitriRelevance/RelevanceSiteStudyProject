namespace RelevanceSiteStudyProject.Data
{
    public class Post
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        // Navigation properties
        public int UserId { get; set; }
        public User User { get; set; } = null!;
        // Foreign key for the category
        public int CategoryId { get; set; }
        public Category Category { get; set; } = null!;
    }
}
