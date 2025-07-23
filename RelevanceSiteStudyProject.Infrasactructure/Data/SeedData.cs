using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace RelevanceSiteStudyProject.Data
{
    public static class SeedData
    {
        public static async Task InitializeAsync(AppDbContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                var role = new IdentityRole("Admin");
                await roleManager.CreateAsync(role);
            }

            if (!await roleManager.RoleExistsAsync("RegularUser"))
            {
                var role = new IdentityRole("RegularUser");
                await roleManager.CreateAsync(role);
            }

            //Seed admin user
            if (await userManager.FindByNameAsync("admin") is null)
            {
                var adminUser = new User
                {
                    UserName = "admin",
                    Email = "admin@site.com",
                    Name = "Admin User",
                    IsAdmin = true,
                    EmailConfirmed = true,
                };
                var result = await userManager.CreateAsync(adminUser, "AdminPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            //Seed regular user
            if (await userManager.FindByNameAsync("user") is null)
            {
                var regularUser = new User
                {
                    UserName = "user",
                    Email = "firstUser@site.com",
                    Name = "Regular User",
                    IsAdmin = false,
                    EmailConfirmed = true,
                };

                var result = await userManager.CreateAsync(regularUser, "UserPassword123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(regularUser, "RegularUser");
                }
            }

            var adminUserInfo = await userManager.Users.FirstOrDefaultAsync(u => u.IsAdmin);
            if (adminUserInfo is null)
                throw new NullReferenceException();
            var regularUserInfo = await userManager.Users.FirstOrDefaultAsync(u => !u.IsAdmin);
            if (regularUserInfo is null)
                throw new NullReferenceException();

            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Id = 1, Name = "Technology" },
                    new Category { Id = 2, Name = "Health" },
                    new Category { Id = 3, Name = "Lifestyle" }
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
                        UserId = adminUserInfo.Id
                    },
                    new Post
                    {
                        Title = "Second Post",
                        Content = "This is the content of the second post.",
                        CategoryId = 2,
                        UserId = regularUserInfo.Id
                    }
                );
            }

            await context.SaveChangesAsync();
        }
    }
}
