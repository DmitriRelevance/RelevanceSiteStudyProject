namespace RelevanceSiteStudyProject.Data
{
    public static class SeedData
    {
        internal static Task InitializeAsync(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Id = 1, Name = "Technology" },
                    new Category { Id = 2, Name = "Health" },
                    new Category { Id = 3, Name = "Lifestyle" }
                );
            }
            if (!context.Users.Any())
            {
                context.Users.AddRange(
                    new User { Id = 1, Username = "admin", Email = "admin@site.com" }
                    );

            }

            if (!context.Posts.Any())
            {
                context.Posts.AddRange(
                    new Post
                    {
                        Title = "First Post",
                        Content = "This is the content of the first post.",
                        CategoryId = 1,
                        UserId = 1
                    },
                    new Post
                    {
                        Title = "Second Post",
                        Content = "This is the content of the second post.",
                        CategoryId = 2,
                        UserId = 1
                    }
                );
            }

            return context.SaveChangesAsync();
        }
    }
}
