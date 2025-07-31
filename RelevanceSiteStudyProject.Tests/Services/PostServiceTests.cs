using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;
using RelevanceSiteStudyProject.Core.Interfaces;
using RelevanceSiteStudyProject.Services.Services;

namespace RelevanceSiteStudyProject.Tests.Services
{
    public class PostServiceTests
    {
        private readonly Mock<IPostRepository> _postRepoMock = new();
        private readonly Mock<ILogRepository> _logRepoMock = new();
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<ILogger<PostService>> _loggerMock = new();

        private readonly PostService _postService;

        public PostServiceTests()
        {
            _userManagerMock = MockUserManager<User>();
            _postService = new PostService(
                _postRepoMock.Object,
                _logRepoMock.Object,
                _loggerMock.Object,
                _userManagerMock.Object
            );
        }

        [Fact]
        public async Task Add_ShouldReturnDto_WhenSuccessful()
        {
            var dto = new PostCreateDto { Title = "Test", Content = "Test Content" };
            var entity = new Post { Id = 1, Title = "Test", Content = "Test Content" };

            _postRepoMock.Setup(r => r.AddAsync(It.IsAny<Post>())).ReturnsAsync(entity);

            var result = await _postService.Add(dto);

            result.Should().NotBeNull();
            result.Title.Should().Be("Test");
            _postRepoMock.Verify(r => r.AddAsync(It.IsAny<Post>()), Times.Once);
        }

        [Fact]
        public async Task GetPosts_ShouldReturnMappedDtos()
        {
            var posts = new List<Post>
        {
            new Post { Id = 1, Title = "Post 1", Content = "Content 1" },
            new Post { Id = 2, Title = "Post 2", Content = "Content 2" }
        };

            _postRepoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(posts);

            var result = await _postService.GetPosts();

            result.Should().HaveCount(2);
            result[0].Title.Should().Be("Post 1");
            result[1].Title.Should().Be("Post 2");
        }

        [Fact]
        public async Task Update_ShouldThrowIfPostNotFound()
        {
            _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Post?)null);

            var dto = new PostDto { Id = 1, Title = "Updated", Content = "Updated" };

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.Update(dto, "user1"));
        }

        [Fact]
        public async Task Update_ShouldThrowIfUserNotFound()
        {
            var post = new Post { Id = 1, UserId = "user1", Title = "Old", Content = "Old" };
            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _userManagerMock.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync((User?)null);

            var dto = new PostDto { Id = 1, Title = "Updated", Content = "Updated" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _postService.Update(dto, "user1"));
        }

        [Fact]
        public async Task Update_ShouldThrowIfUserUnauthorized()
        {
            var post = new Post { Id = 1, UserId = "owner", Title = "Old", Content = "Old" };
            var user = new User { Id = "user1" };

            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _userManagerMock.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

            var dto = new PostDto { Id = 1, Title = "Updated", Content = "Updated" };

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _postService.Update(dto, "user1"));
        }

        [Fact]
        public async Task Update_ShouldUpdatePost_WhenAuthorized()
        {
            var post = new Post { Id = 1, UserId = "user1", Title = "Old", Content = "Old" };
            var user = new User { Id = "user1" };

            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _userManagerMock.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);
            _userManagerMock.Setup(um => um.IsInRoleAsync(user, "Admin")).ReturnsAsync(false);

            var dto = new PostDto { Id = 1, Title = "Updated", Content = "Updated" };

            await _postService.Update(dto, "user1");

            _postRepoMock.Verify(r => r.UpdateAsync(It.Is<Post>(p => p.Title == "Updated" && p.Content == "Updated")), Times.Once);
        }

        [Fact]
        public async Task Delete_ShouldThrowIfPostNotFound()
        {
            _postRepoMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Post?)null);

            await Assert.ThrowsAsync<KeyNotFoundException>(() => _postService.Delete(1, "user1"));
        }

        [Fact]
        public async Task Delete_ShouldThrowIfUserNotFound()
        {
            var post = new Post { Id = 1, UserId = "user1" };
            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _userManagerMock.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync((User?)null);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _postService.Delete(1, "user1"));
        }

        [Fact]
        public async Task Delete_ShouldThrowIfUserUnauthorized()
        {
            var post = new Post { Id = 1, UserId = "owner" };
            var user = new User { Id = "user1", IsAdmin = false };

            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _userManagerMock.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _postService.Delete(1, "user1"));
        }

        [Fact]
        public async Task Delete_ShouldDeletePost_WhenAuthorized()
        {
            var post = new Post { Id = 1, UserId = "user1" };
            var user = new User { Id = "user1", IsAdmin = false };

            _postRepoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(post);
            _userManagerMock.Setup(um => um.FindByIdAsync("user1")).ReturnsAsync(user);

            await _postService.Delete(1, "user1");

            _postRepoMock.Verify(r => r.DeleteAsync(post), Times.Once);
        }

        // Helper to mock UserManager<TUser>
        private static Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
        {
            var store = new Mock<IUserStore<TUser>>();
            return new Mock<UserManager<TUser>>(
                store.Object, null, null, null, null, null, null, null, null);
        }
    }

}
