using Microsoft.EntityFrameworkCore;
using Stotele.Server.Models.ApplicationDbContexts;
using Stotele.Server.Models;
using DotNetEnv;
using Stripe;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.DataProtection;

namespace Stotele.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Load environment variables from .env if present
            Env.Load();

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            builder.Services.AddDataProtection()
            .PersistKeysToFileSystem(new DirectoryInfo("/keys"))
            .SetApplicationName("ZolesStotele");

            // Session state configuration
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
                options.Cookie.SameSite = SameSiteMode.Lax; // or Strict if no cross-site needed
                options.Cookie.SecurePolicy = CookieSecurePolicy.None; // allow HTTP
                options.IdleTimeout = TimeSpan.FromMinutes(30);
            });



            // If your frontend and backend are served from the same domain in production,
            // you can remove or simplify CORS entirely:
            // builder.Services.AddCors(options =>
            // {
            //     options.AddPolicy("AllowAll",
            //         policy => policy
            //             .AllowAnyOrigin()
            //             .AllowAnyMethod()
            //             .AllowAnyHeader());
            // });

            var dbPassword = Environment.GetEnvironmentVariable("DB_PASSWORD");
            if (string.IsNullOrEmpty(dbPassword))
            {
                throw new InvalidOperationException("DB_PASSWORD environment variable is not set.");
            }

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                .Replace("${DB_PASSWORD}", dbPassword);

            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(connectionString));

            // Stripe configuration
            var stripeSecretKey = Environment.GetEnvironmentVariable("STRIPE_SECRET_KEY");
            if (string.IsNullOrEmpty(stripeSecretKey))
            {
                throw new InvalidOperationException("STRIPE_SECRET_KEY environment variable is not set.");
            }

            builder.Services.Configure<StripeSettings>(builder.Configuration.GetSection("Stripe"));
            StripeConfiguration.ApiKey = stripeSecretKey;
            builder.Configuration["Stripe:SecretKey"] = stripeSecretKey;

            var stripeWebhookSecret = Environment.GetEnvironmentVariable("STRIPE_WEBHOOK_SECRET");
            if (string.IsNullOrEmpty(stripeWebhookSecret))
            {
                throw new InvalidOperationException("STRIPE_WEBHOOK_SECRET environment variable is not set.");
            }

            builder.Configuration["Stripe:WebhookSecret"] = stripeWebhookSecret;

            // Swagger (optional)
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

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

            builder.Services.AddSingleton<EmailService>();

            var app = builder.Build();

            // Test the database connection (optional)
            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                try
                {
                    if (dbContext.Database.CanConnect())
                        Console.WriteLine("Database connection successful.");
                    else
                        Console.WriteLine("Database connection could not be established.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Database connection failed: {ex.Message}");
                }
            }

            app.UseDefaultFiles();
            app.UseStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Stotele API V1");
                options.RoutePrefix = "swagger";
            });

            // Comment out UseHttpsRedirection if not terminating TLS at the container level
            // app.UseHttpsRedirection();

            // If you removed CORS, you don't need app.UseCors("AllowAll")
            // If needed (different domain scenario), configure cors with specific origin and allow credentials.
            // app.UseCors("AllowAll");

            app.UseSession();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();
            app.MapFallbackToFile("/index.html");

            app.Run();
        }
    }
}
