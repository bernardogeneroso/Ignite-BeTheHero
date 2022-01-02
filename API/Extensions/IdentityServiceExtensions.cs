using System.Text;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
namespace API.Extensions;

public static class IdentityServiceExtensions
{
  public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration config)
  {
    services.AddAuthentication(cfg =>
    {
      cfg.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
      cfg.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
   .AddJwtBearer(cfg =>
   {
     cfg.TokenValidationParameters = new TokenValidationParameters()
     {
       ValidateIssuer = true,
       ValidIssuer = config["JWT:Issuer"],
       ValidateAudience = true,
       ValidAudience = config["JWT:Audience"],
       ValidateIssuerSigningKey = true,
       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JWT:TokenKey"])),
       ClockSkew = TimeSpan.Zero

     };
   });
    services.AddAuthorization();

    services.AddScoped<TokenService>();

    return services;
  }
}
