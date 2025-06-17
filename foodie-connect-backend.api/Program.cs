using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using foodie_connect_backend.Data;
using foodie_connect_backend.Extensions;
using foodie_connect_backend.Extensions.DI;
using foodie_connect_backend.Modules.DishCategories;
using foodie_connect_backend.Modules.Dishes;
using foodie_connect_backend.Modules.Dishes.Hub;
using foodie_connect_backend.Modules.DishReviews;
using foodie_connect_backend.Modules.GeoCoder;
using foodie_connect_backend.Modules.Heads;
using foodie_connect_backend.Modules.Promotions;
using foodie_connect_backend.Modules.RestaurantReviews;
using foodie_connect_backend.Modules.Restaurants;
using foodie_connect_backend.Modules.Sessions;
using foodie_connect_backend.Modules.Socials;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Modules.Users;
using foodie_connect_backend.Modules.Verification;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.Shared.Converters;
using foodie_connect_backend.Shared.Policies;
using foodie_connect_backend.Shared.Policies.Dish;
using foodie_connect_backend.Shared.Policies.EmailConfirmed;
using foodie_connect_backend.Shared.Policies.Restaurant;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;
using Swashbuckle.AspNetCore.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => 
    { 
        options.SuppressMapClientErrors = true; 
    })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new EnumOrStructJsonConverterFactory());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        options.JsonSerializerOptions.NumberHandling = JsonNumberHandling.AllowNamedFloatingPointLiterals;
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    // XML comments configuration
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    options.IncludeXmlComments(xmlPath);
    options.AddSignalRSwaggerGen();

    // Add examples support
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    ));

// Register services
builder.Services.AddThirdPartyDependencies();
builder.Services.AddSignalR();
builder.Services.AddAuthenticationServices();
builder.Services.AddAuthorizationHandlers();
builder.Services.AddRestaurantServices();
builder.Services.AddDishServices();

// Register custom design patterns (Factory Method and Decorator)
builder.Services.AddDesignPatterns();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder =>
    {
        corsBuilder.WithOrigins(builder.Configuration["FRONTEND_URL"]!.Split(","))
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Seed roles if not exist
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Head", "User" };
    foreach (var role in roles)
        if (!await roleManager.RoleExistsAsync(role))
            await roleManager.CreateAsync(new IdentityRole(role));
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("CorsPolicy");

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.MapHub<DishViewersHub>("/dishViewersHub");

app.Run();

namespace foodie_connect_backend
{
    public partial class Program
    {
    }
}
