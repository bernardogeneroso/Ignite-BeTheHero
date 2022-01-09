using Application.Interfaces;
using Application.Records;
using Infrastructure.Image;
using Infrastructure.Security;
using MediatR;
using Microsoft.Extensions.Azure;
using MongoDB.Driver;
using Persistence;

namespace API.Extensions;

public static class ApplicationServiceExtensions
{
  public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
  {
    services.AddSwaggerGen();

    services.AddAzureClients(builder =>
    {
      builder.AddBlobServiceClient(config["Storage:ConnectionString"]);
    });
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
                    .WithOrigins("http://localhost:3000");
              });
        });

    services.AddMediatR(typeof(List.Handler).Assembly);
    services.AddScoped<IUserAccessor, UserAccessor>();
    services.AddScoped<IStorageAccessor, StorageAccessor>();

    return services;
  }
}
