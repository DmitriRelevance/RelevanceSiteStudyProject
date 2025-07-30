using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;
using RelevanceSiteStudyProject.Core.Interfaces;
using RelevanceSiteStudyProject.Core.Services;
using RelevanceSiteStudyProject.Infrasactructure.Data;
using RelevanceSiteStudyProject.Services.Services;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register services
builder.Services.AddDbContext<RelevanceSiteStudyProject.Infrasactructure.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<IPostService, PostService>();
builder.Services.AddScoped<IJWTTokenService, JWTTokenService>();


builder.Services.AddLogging(config =>
{
    config.AddConsole();
    config.AddDebug();
    config.SetMinimumLevel(LogLevel.Information);
});

var config = builder.Configuration;

builder.Services.AddAuthorization();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = config["Jwt:Issuer"],
            ValidAudience = config["Jwt:Audience"],
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key")))
        };


        // TEMP: log failures for debugging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine("AUTH FAILED: " + context.Exception.Message);
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("TOKEN VALIDATED for: " + context.Principal?.Identity?.Name);
                return Task.CompletedTask;
            }
        };
    });

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseAuthentication();
app.UseMiddleware<JwtMiddleware>();
app.UseAuthorization();

// Map routes to PostService
app.MapGet("/posts", async (IPostService postService) => await postService.GetPosts());
app.MapPost("/posts", async (
    HttpContext context,
    IPostService postService,
    PostCreateDto post) =>
{
    var user = context.User;
    if (user.Identity is not { IsAuthenticated: true })
    {
        return Results.Unauthorized();
    }

    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
        return Results.Unauthorized();
    }

    if(userId.Equals(post.UserId, StringComparison.OrdinalIgnoreCase) is false)
    {
        // User is trying to create a post with a different userId than their own
        return Results.Forbid();
    }

    try
    {
        var addedPost = await postService.Add(post);
        return Results.Ok(addedPost);
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

});

app.MapPut("/posts/{id}", async (
    HttpContext context,
    IPostService postService,
    PostDto post) =>
{
    var user = context.User;
    if (user.Identity is not { IsAuthenticated: true })
    {
        return Results.Unauthorized();
    }

    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
        return Results.Unauthorized();
    }

    try
    {
        await postService.Update(post, userId);
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
});

app.MapDelete("/posts/{id}", async (
    HttpContext context,
    IPostService postService,
    int id) =>
{
    var user = context.User;
    if (user.Identity is not { IsAuthenticated: true })
    {
        return Results.Unauthorized();
    }

    var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    if (string.IsNullOrEmpty(userId))
    {
        return Results.Unauthorized();
    }

    try
    {
        await postService.Delete(id, userId);
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
});



app.MapPost("/login", async (LoginDto login, UserManager<User> userManager, IJWTTokenService jwtTokenService, IConfiguration config) =>
{
    var user = await userManager.FindByEmailAsync(login.Email);
    if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
    {
        return Results.Unauthorized();
    }

    var tokenString = jwtTokenService.GenerateJwtToken(user);

    // Validate the token immediately after generation
    var validationResult = jwtTokenService.ValidateJwtToken(tokenString);
    if (validationResult == null)
    {
        return Results.Problem("Token validation failed immediately after generation.");
    }

    return Results.Ok(tokenString);
});


app.Run();