using Application.Records;
using MediatR;
using MongoDB.Driver;
using Persistence;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
  {
    services.AddSwaggerGen();

    services.AddSingleton<IMongoClient>(s => new MongoClient(config["MongoDb:ConnectionString"]));
    services.AddScoped(s => new MongoDbContext(s.GetRequiredService<IMongoClient>(), config["MongoDb:DatabaseName"]));

    services.AddCors(opt =>
        {
          opt.AddPolicy("CorsPolicy", policy =>
              {
                policy
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .WithExposedHeaders("WWW-Authenticate", "Pagination")
                        .WithOrigins("http://localhost:3000");
              });
        });

    services.AddMediatR(typeof(List.Handler).Assembly);

    return services;
  }
}
