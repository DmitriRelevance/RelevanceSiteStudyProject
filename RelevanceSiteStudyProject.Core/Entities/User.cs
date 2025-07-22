namespace RelevanceSiteStudyProject.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool IsAdmin { get; set; } = false;

        public ICollection<Post> Posts { get; internal set; }
    }
}
