using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models.ApplicationDbContexts;
using Stotele.Server.Models;
using DotNetEnv;
using Stripe;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Stotele.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Env.Load();


            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // For session state
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true; // Keeps the cookie secure from client-side JavaScript
                options.Cookie.IsEssential = true; // Required for GDPR compliance
                options.Cookie.SameSite = SameSiteMode.None; // Allows cross-origin cookies
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Ensures cookies are sent only over HTTPS
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowReactApp",
                    builder => builder.WithOrigins("https://localhost:5173", "https://localhost:5210")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });


            // Read the environment variable
            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            if (string.IsNullOrEmpty(dbPassword))
            {
                throw new InvalidOperationException("DB_PASSWORD environment variable is not set.");
            }

            // Build the connection string dynamically
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                .Replace("${DB_PASSWORD}", dbPassword);

            // Register ApplicationDbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));


            // Configure Stripe
            var stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
            if (string.IsNullOrEmpty(stripeSecretKey))
            {
                throw new InvalidOperationException("STRIPE_SECRET_KEY environment variable is not set.");
            }

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = stripeSecretKey;

            Console.WriteLine($"Stripe Secret Key: {stripeSecretKey}");
            // Replace placeholder in configuration
            builder.Configuration["Stripe:SecretKey"] = stripeSecretKey;

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // JWT Authentication
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET");
            if (string.IsNullOrEmpty(jwtSecret) || jwtSecret.Length < 16)
            {
                throw new InvalidOperationException("JWT_SECRET is not set or is less than 128 bits.");
            }
            builder.Configuration["JwtSettings:Key"] = jwtSecret;

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                        ValidAudience = builder.Configuration["JwtSettings:Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]))
                    };
                });



            var app = builder.Build();

            // Test the database connection
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    dbContext.Database.CanConnect();
                    Console.WriteLine("Database connection successful.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database connection failed: {ex.Message}");
                }
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stotele API V1");
                    options.RoutePrefix = "swagger"; // Serve Swagger UI at /swagger/
                });
            }
            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");
            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
