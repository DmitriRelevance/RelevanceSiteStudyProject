using Microsoft.EntityFrameworkCore;
using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register services
builder.Services.AddDbContext<RelevanceSiteStudyProject.Infrasactructure.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<RelevanceSiteStudyProject.Core.Interfaces.IPostService, RelevanceSiteStudyProject.Services.Services.PostService>();

builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.SetMinimumLevel(LogLevel.Information);
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Map routes to PostService
app.MapGet("/posts", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService) => await postService.GetPosts());
app.MapPost("/posts", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService, PostCreateDto post) => await postService.Add(post));
//app.MapPut("/posts/{id}", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService, int id, PostDto post) => await postService.Update(post, id));
//app.MapDelete("/posts/{id}", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService, int id) => await postService.Delete(id));

app.Run();


app.Run();