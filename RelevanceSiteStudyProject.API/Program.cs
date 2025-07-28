using Microsoft.EntityFrameworkCore;
using RelevanceSiteStudyProject.API;
using RelevanceSiteStudyProject.API.Models;
using RelevanceSiteStudyProject.Core.Entities;
using static System.Runtime.InteropServices.JavaScript.JSType;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<TodoDb>(opt => opt.UseInMemoryDatabase("TodoList"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register services
builder.Services.AddDbContext<RelevanceSiteStudyProject.Infrasactructure.Data.AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddScoped<RelevanceSiteStudyProject.Core.Interfaces.IPostService, RelevanceSiteStudyProject.Services.Services.PostService>();
builder.Services.AddLogging();

var app = builder.Build();

// Map routes to PostService
app.MapGet("/posts", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService) => await postService.GetPosts());
app.MapPost("/posts", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService, Post post) => await postService.Add(post));
//app.MapPut("/posts/{id}", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService, int id, Post post) => await postService.Update(post, id));
//app.MapDelete("/posts/{id}", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService, int id) => await postService.Delete(id));

app.Run();


app.Run();