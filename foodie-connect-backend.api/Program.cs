using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using foodie_connect_backend.Data;
using foodie_connect_backend.GeoCoder;
using foodie_connect_backend.Heads;
using foodie_connect_backend.Restaurants;
using foodie_connect_backend.Sessions;
using foodie_connect_backend.Shared.Classes.Errors;
using foodie_connect_backend.SocialLinks;
using foodie_connect_backend.Uploader;
using foodie_connect_backend.Users;
using foodie_connect_backend.Verification;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .ConfigureApiBehaviorOptions(options => { options.SuppressMapClientErrors = true; })
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
;

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

// Identity Server
builder.Services.AddAuthentication()
    .AddCookie(IdentityConstants.ApplicationScheme, options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;
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

builder.Services.AddIdentityCore<User>(options => { options.User.RequireUniqueEmail = true; })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddApiEndpoints();

// Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddFluentEmail("verify@account.foodie.town", "Verify your email address")
    .AddMailtrapSender(
        builder.Configuration["MAILTRAP_USERNAME"],
        builder.Configuration["MAILTRAP_PASSWORD"],
        builder.Configuration["MAILTRAP_HOST"],
        int.TryParse(builder.Configuration["MAILTRAP_PORT"], out var port) ? port : null);
builder.Services.AddScoped<IGeoCoderService, ReverseGeoCoder>(sp =>
    new ReverseGeoCoder(builder.Configuration["GOOGLE_APIKEY"]!));
builder.Services.AddScoped<IUploaderService, CloudinaryUploader>();
builder.Services.AddScoped<HeadsService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<SessionsService>();
builder.Services.AddScoped<VerificationService>();
builder.Services.AddScoped<RestaurantsService>();
builder.Services.AddScoped<SocialLinksService>();

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

AppContext.SetSwitch("Npgsql.DisableDateTimeInfinityConversions", true);
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