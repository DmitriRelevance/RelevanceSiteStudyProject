using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RelevanceSiteStudyProject.Core.DTOs;
using RelevanceSiteStudyProject.Core.Entities;
using RelevanceSiteStudyProject.Core.Interfaces;
using RelevanceSiteStudyProject.Infrasactructure.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Register services
builder.Services.AddDbContext<RelevanceSiteStudyProject.Infrasactructure.Data.AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped<RelevanceSiteStudyProject.Core.Interfaces.IPostService, RelevanceSiteStudyProject.Services.Services.PostService>();

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
            IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(config["Jwt:Key"]))
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
app.UseAuthorization();

// Map routes to PostService
app.MapGet("/posts", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService) => await postService.GetPosts());
app.MapPost("/posts", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService, PostCreateDto post) => await postService.Add(post));
app.MapPut("/posts/{id}", async (
    HttpContext context,
    IPostService postService,
    int id,
    PostDto post) =>
{

    var authHeader = context.Request.Headers["Authorization"].ToString();
    if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
    {
        return Results.Unauthorized();
    }

    var token = authHeader["Bearer ".Length..].Trim();

    var userPrincipal = ValidateJwtToken(token, config);
    if (userPrincipal == null)
    {
        return Results.Unauthorized();
    }

    var userId = userPrincipal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

    //// Ensure token is valid (auth is required)
    //var user = context.User;
    //if (user.Identity is not { IsAuthenticated: true })
    //{
    //    return Results.Unauthorized();
    //}

    //// Extract user info from token (e.g., sub, nameidentifier)
    //string userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    //var isAdmin = user.IsInRole("Admin");

    if (string.IsNullOrEmpty(userId))
    {
        return Results.Unauthorized();
    }

    // Call service method with user info
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


    ClaimsPrincipal? ValidateJwtToken(string token, IConfiguration config)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
});
//app.MapDelete("/posts/{id}", async (RelevanceSiteStudyProject.Core.Interfaces.IPostService postService, int id) => await postService.Delete(id));


app.MapPost("/login", async (LoginDto login, UserManager<User> userManager, IConfiguration config) =>
{
    var user = await userManager.FindByEmailAsync(login.Email);
    if (user == null || !await userManager.CheckPasswordAsync(user, login.Password))
    {
        return Results.Unauthorized();
    }

    var claims = new[]
    {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName)
    };

    var tokenString = GenerateJwtToken(user, config);

    var validationResult = ValidateJwtToken(tokenString, config);
    if (validationResult == null)
    {
        return Results.Problem("Token validation failed immediately after generation.");
    }

    return Results.Ok(tokenString);

    //TODO: Move this method to a separate class or service for better organization
    string GenerateJwtToken(User user, IConfiguration config)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.Id),
        new Claim(ClaimTypes.Email, user.Email),
        new Claim(ClaimTypes.Name, user.UserName)
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: config["Jwt:Issuer"],
            audience: config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }


    ClaimsPrincipal? ValidateJwtToken(string token, IConfiguration config)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(config["Jwt:Key"]);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = config["Jwt:Issuer"],
                ValidAudience = config["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(key)
            }, out SecurityToken validatedToken);

            return principal;
        }
        catch
        {
            return null;
        }
    }
});


app.Run();