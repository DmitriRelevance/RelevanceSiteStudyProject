using System.ComponentModel.DataAnnotations;

namespace RelevanceSiteStudyProject.API.Filters
{
    public class ValidationFilter<T> : IEndpointFilter where T : class
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var dto = context.Arguments.OfType<T>().FirstOrDefault();
            if (dto is null)
                return Results.BadRequest("Invalid request body.");

            var validationContext = new ValidationContext(dto);
            var results = new List<ValidationResult>();

            if (!Validator.TryValidateObject(dto, validationContext, results, validateAllProperties: true))
            {
                var errorDict = results
                    .GroupBy(e => e.MemberNames.FirstOrDefault() ?? "")
                    .ToDictionary(g => g.Key, g => g.Select(r => r.ErrorMessage).ToArray());

                return Results.ValidationProblem(errorDict);
            }

            return await next(context);
        }
    }

}
