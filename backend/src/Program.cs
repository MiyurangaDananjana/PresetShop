using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ProjectX.API.Data;
using ProjectX.API.Service;

var builder = WebApplication.CreateBuilder(args);

// Configure Database Connection
// Priority: 1) Environment variables (for Docker), 2) appsettings.json ConnectionStrings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// If ConnectionString not in appsettings, build from environment variables
if (string.IsNullOrEmpty(connectionString))
{
    connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost"};" +
                      $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432"};" +
                      $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "preset_shop"};" +
                      $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "sa"};" +
                      $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "abc"}";
}
// Override with environment variables if they exist (Docker deployment)
else if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable("POSTGRES_HOST")))
{
    connectionString = $"Host={Environment.GetEnvironmentVariable("POSTGRES_HOST")};" +
                      $"Port={Environment.GetEnvironmentVariable("POSTGRES_PORT") ?? "5432"};" +
                      $"Database={Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "preset_shop"};" +
                      $"Username={Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "sa"};" +
                      $"Password={Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "abc"}";
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

// Register services
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPresetService, PresetService>();
builder.Services.AddScoped<IPurchaseService, PurchaseService>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["JwtSettings:SecretKey"] ?? "HGY5678YTR432REWDSTYUHNB456789UD!";
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"] ?? "PresetShopAPI";
var jwtAudience = builder.Configuration["JwtSettings:Audience"] ?? "PresetShopClient";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtIssuer,
        ValidAudience = jwtAudience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
    };
});

builder.Services.AddAuthorization();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://frontend:3000")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Automatically apply database migrations on startup
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogInformation("Applying database migrations...");
        context.Database.Migrate();
        logger.LogInformation("Database migrations applied successfully");

        // Seed default data
        logger.LogInformation("Seeding default data...");
        await DbInitializer.SeedDefaultAdminAsync(context);
        await DbInitializer.SeedDefaultCategoriesAsync(context);
        logger.LogInformation("Default data seeded successfully");
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "An error occurred while migrating the database");
        throw;
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Enable CORS
app.UseCors("AllowFrontend");

// Serve static files (uploaded images)
app.UseStaticFiles();

app.UseHttpsRedirection();

// Authentication & Authorization middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// Health check endpoint
app.MapGet("/api/health", () => Results.Ok(new { status = "healthy", timestamp = DateTime.UtcNow }));

app.Run();
