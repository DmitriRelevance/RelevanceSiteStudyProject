using RelevanceSiteStudyProject;
using RelevanceSiteStudyProject.ViewModels;

namespace RelevanceSiteStudyProject.Helpers
{
    public static class MappingExtensions
    {
        public static Data.User ToDataModel(User user)
        {
            if (user == null)
            {
                return new();
            }
            return new Data.User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
            };
        }
        public static User ToViewModel(Data.User user)
        {
            if (user == null)
            {
                return new();
            }
            return new User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                IsAdmin = user.IsAdmin,
            };
        }
        public static Data.Category ToDataModel(Category category)
        {
            if (category == null)
            {
                return new Data.Category();
            }
            return new Data.Category
            {
                Id = category.Id,
                Name = category.Name
            };
        }
        public static Category ToViewModel(Data.Category category)
        {
            if (category == null)
            {
                return new Category();
            }
            return new Category
            {
                Id = category.Id,
                Name = category.Name
            };
        }
        public static Data.Post ToDataModel(Post post)
        {
            if (post == null)
            {
                return new Data.Post();
            }
            return new Data.Post
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                UserId = post.UserId,
                //User = ToDataModel(post.User),
                CategoryId = post.CategoryId,
                //Category = ToDataModel(post.Category)
            };
        }
        public static Post ToViewModel(Data.Post post)
        {
            if (post == null)
            {
                return new Post();
            }
            return new Post
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                CreatedAt = post.CreatedAt,
                UpdatedAt = post.UpdatedAt,
                UserId = post.UserId,
                //User = ToViewModel(post.User),
                CategoryId = post.CategoryId,
                //Category = ToViewModel(post.Category)
            };
        }

        public static IList<TOutput> ToDataModel<TInput, TOutput>(
            IList<TInput> input,
            Func<TInput, TOutput> mapFunc)
            where TOutput : class
            where TInput : class
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Posts cannot be null.");
            }
            if (mapFunc == null)
            {
                throw new ArgumentNullException(nameof(mapFunc), "Mapping function cannot be null.");
            }

            return input.Select(mapFunc).ToList();
        }

        public static IList<TOutput> ToViewModel<TInput, TOutput>(
            IList<TInput> input,
            Func<TInput, TOutput> mapFunc)
            where TInput : class
            where TOutput : class
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input), "Posts cannot be null.");
            }
            if (mapFunc == null)
            {
                throw new ArgumentNullException(nameof(mapFunc), "Mapping function cannot be null.");
            }
            return input.Select(mapFunc).ToList();
        }
    }
}
