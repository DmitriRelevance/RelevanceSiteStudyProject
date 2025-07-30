namespace RelevanceSiteStudyProject.API.Helpers
{
    public static class ServiceCallHandler
    {
        public static async Task<IResult> HandleActionAsync(Func<Task> action)
        {
            try
            {
                await action();
                return Results.NoContent();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }

        public static async Task<IResult> HandleActionWithResultAsync<TResult>(Func<Task<TResult>> action)
        {
            try
            {
                var result = await action();
                return Results.Ok(result);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Results.Forbid();
            }
            catch (KeyNotFoundException ex)
            {
                return Results.NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return Results.Problem(ex.Message);
            }
        }
    }
}
