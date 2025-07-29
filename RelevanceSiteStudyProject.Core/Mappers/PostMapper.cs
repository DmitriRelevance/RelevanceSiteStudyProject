using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;

namespace RelevanceSiteStudyProject.Core.Mappers
{
    public static class PostMapper
    {
        public static Post ToEntity(this PostDto postCreateDto)
        {
            if (postCreateDto == null)
                throw new ArgumentNullException(nameof(postCreateDto));
            return new Post
            {
                Title = postCreateDto.Title,
                Content = postCreateDto.Content,
                UserId = postCreateDto.UserId,
                CategoryId = postCreateDto.CategoryId
            };
        }
        public static PostDto ToDto(this Post post)
        {
            if (post == null)
                throw new ArgumentNullException(nameof(post));
            return new PostDto
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                UserId = post.UserId,
                CategoryId = post.CategoryId
            };
        }

        public static IList<TOutput> ToDto<TInput, TOutput>(IList<TInput> input, Func<TInput, TOutput> mapFunc)
           {
            if (input == null)
                throw new ArgumentNullException(nameof(input), "Input cannot be null.");
            if (mapFunc == null)
                throw new ArgumentNullException(nameof(mapFunc), "Mapping function cannot be null.");

            return input.Select(mapFunc).ToList();
        }
    }
}
