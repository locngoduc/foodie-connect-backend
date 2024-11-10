using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using foodie_connect_backend.Data;
using foodie_connect_backend.Modules.Dishes;
using foodie_connect_backend.Modules.DishReviews;
using foodie_connect_backend.Modules.GeoCoder;
using foodie_connect_backend.Modules.Heads;
using foodie_connect_backend.Modules.Restaurants;
using foodie_connect_backend.Modules.Sessions;
using foodie_connect_backend.Modules.Socials;
using foodie_connect_backend.Modules.Uploader;
using foodie_connect_backend.Modules.Users;
using foodie_connect_backend.Modules.Verification;
using foodie_connect_backend.Shared.Classes.Errors;
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
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
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

    // Add examples support
    options.ExampleFilters();
});
builder.Services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

// Identity Server
builder.Services.AddAuthentication()
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = builder.Configuration["ENVIRONMENT"] == "production" ? CookieSecurePolicy.Always : CookieSecurePolicy.None;
        options.Cookie.SameSite = builder.Configuration["ENVIRONMENT"] == "production" ? SameSiteMode.None : SameSiteMode.Lax;;
        options.Cookie.Name = "access_token";
        options.ExpireTimeSpan = TimeSpan.FromDays(14);
        options.Events = new CookieAuthenticationEvents
        {
            OnRedirectToLogin = context =>
            {
                context.Response.StatusCode = 401;
                context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(AuthError.NotAuthenticated().ToJson()));
                return Task.CompletedTask;
            },
            OnRedirectToAccessDenied = context =>
            {
                context.Response.StatusCode = 403;
                context.Response.Body.WriteAsync(Encoding.UTF8.GetBytes(AuthError.NotAuthorized().ToJson()));
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RestaurantOwner", policy =>
        policy.Requirements.Add(new RestaurantOwnerRequirement()));
    
    options.AddPolicy("DishOwner", policy => 
        policy.Requirements.Add(new DishOwnerRequirement()));
    
    options.AddPolicy("EmailVerified", policy =>
        policy.Requirements.Add(new EmailConfirmedRequirement()));
});

builder.Services.AddIdentityCore<User>(options => { options.User.RequireUniqueEmail = true; })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        x => x.UseNetTopologySuite()
    ));


builder.Services.AddFluentEmail("verify@account.foodie.town", "Verify your email address")
    .AddMailtrapSender(
        builder.Configuration["MAILTRAP_USERNAME"],
        builder.Configuration["MAILTRAP_PASSWORD"],
        builder.Configuration["MAILTRAP_HOST"],
        int.TryParse(builder.Configuration["MAILTRAP_PORT"], out var port) ? port : null);

builder.Services.AddScoped<IGeoCoderService, ReverseGeoCoder>(_ =>
    new ReverseGeoCoder(builder.Configuration["GOOGLE_APIKEY"]!));
builder.Services.AddScoped<IUploaderService, CloudinaryUploader>();
builder.Services.AddScoped<HeadsService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<SessionsService>();
builder.Services.AddScoped<VerificationService>();
builder.Services.AddScoped<RestaurantsService>();
builder.Services.AddScoped<SocialsService>();
builder.Services.AddScoped<DishesService>();
builder.Services.AddScoped<DishReviewsService>();
builder.Services.AddScoped<IAuthorizationHandler, RestaurantOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, DishOwnerHandler>();
builder.Services.AddScoped<IAuthorizationHandler, EmailConfirmedHandler>();
builder.Services.AddSingleton<IAuthorizationMiddlewareResultHandler, AuthorizationResponseTransformer>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", corsBuilder =>
    {
        corsBuilder.WithOrigins(builder.Configuration["FRONTEND_URL"]!)
            .AllowCredentials()
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

var app = builder.Build();

// Create roles
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

app.Run();

public partial class Program
{
}