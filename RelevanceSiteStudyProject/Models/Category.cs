namespace RelevanceSiteStudyProject.Data
{
    public class Category
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public ICollection<Post> Posts { get; internal set; }
    }
}
