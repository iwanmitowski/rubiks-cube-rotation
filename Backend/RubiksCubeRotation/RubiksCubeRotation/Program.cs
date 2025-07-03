
using RubiksCubeRotation.Middlewares;

using RubiksCubeServices;

namespace RubiksCubeRotation
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddSingleton<IRubiksCubeService, RubiksCubeService>(); // Transient later

            var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();
            if (allowedOrigins is null || allowedOrigins.Length == 0)
            {
                throw new InvalidOperationException("AllowedOrigins is not configured in appsettings.");
            }

            builder.Services.AddCors(options =>
            {
                options.AddPolicy(name: "AllowSpecificOrigins",
                    policy =>
                    {
                        policy.WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });

            var app = builder.Build();

            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseHttpsRedirection();
            app.UseCors("AllowSpecificOrigins");

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
