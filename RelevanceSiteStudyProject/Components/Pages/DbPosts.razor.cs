using RelevanceSiteStudyProject.ViewModels;
using static RelevanceSiteStudyProject.Components.Pages.Notification;

namespace RelevanceSiteStudyProject.Components.Pages
{
    public partial class DbPosts
    {
        private bool isLoading = true;
        private IList<Post> posts = new List<Post>();
        private Data.User? currentUser;
        private Post postModel = new();
        private bool isEditing = false;
        private Post? postToEdit = null;
        private bool showNotification => this.Notification is not null;
        private NotificationInfo? Notification { get; set; } = null;

        private bool requiresPostUpdate = true;

        protected override async Task OnInitializedAsync()
        {
            await UpdateAuthorizationState();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            // if (firstRender && currentUser is null)
            // {
            //     Navigation.NavigateTo("/login", true);
            // }

            if (firstRender)
            {
            }

            if (requiresPostUpdate)
            {
                await GetPosts();
                requiresPostUpdate = false;
            }

            await UpdateAuthorizationState();
        }

        private async Task UpdateAuthorizationState()
        {
            var user = HttpContextAccessor.HttpContext?.User;

            if (user is not null)
            {
                currentUser = await UserManager.GetUserAsync(user);
            }
        }

        private async Task CreatePost()
        {
            if (currentUser == null) return;

            var newPost = new Post
            {
                Title = postModel.Title,
                Content = postModel.Content,
                UserId = currentUser.Id,
                CategoryId = 1
            };
            var addedPost = await _postService.Add(newPost);
            //Update the posts list with the new post
            requiresPostUpdate = true;
            postModel = new Post();
            Notification = new NotificationInfo { Type = NotificationInfoType.Info, Message = "Successfully added a post!" };
        }

        /// <summary>
        /// Refresh the posts list after editing
        /// </summary>
        /// <returns></returns>
        private async Task GetPosts()
        {
            try
            {
                isLoading = true;
                posts = await _postService.GetPosts();
                _logger.LogInformation($"Loaded {posts.Count} posts.");
                //statusMessage = (true, "Posts loaded successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error loading posts: {ex.Message}");
                Notification = new NotificationInfo { Type = NotificationInfoType.Error, Message = "Failed to create post. Please try again." };
            }
            finally
            {
                isLoading = false;
                StateHasChanged();
            }
        }

        private async Task EditPost(Post post)
        {
            if (currentUser == null || (!currentUser.IsAdmin && post.UserId != currentUser.Id))
            {
                return; // Only allow editing if the user is an admin or the post belongs to the user
            }

            isEditing = true;

            postToEdit = new Post
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                CategoryId = post.CategoryId
            };

            StateHasChanged();
        }

        private async Task SaveEdit()
        {
            if (currentUser is null || postToEdit is null)
            {
                return;
            }

            try
            {
                await _postService.Update(postToEdit, currentUser);
                requiresPostUpdate = true;
                isEditing = false;
                postToEdit = null;
                Notification = new NotificationInfo { Type = NotificationInfoType.Info, Message = "Post updated successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating post: {ex.Message}");
                Notification = new NotificationInfo { Type = NotificationInfoType.Error, Message = "Failed to update post. Please try again." };
            }
        }

        private async Task CancelEdit()
        {
            isEditing = false;
            postToEdit = null;
            StateHasChanged();
        }

        private async Task DeletePost(Post post)
        {
            try
            {
                if (currentUser is null)
                    throw new InvalidOperationException("User must be logged in to delete a post.");

                await _postService.Delete(post, currentUser);
                Notification = new NotificationInfo { Type = NotificationInfoType.Info, Message = "Post deleted successfully." };
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting post: {ex.Message}");
                Notification = new NotificationInfo { Type = NotificationInfoType.Error, Message = "Failed to delete post. Please try again." };
            }

            requiresPostUpdate = true;
            StateHasChanged();
        }
    }
}
