using System.Security.Claims;
using Domain;
using Microsoft.AspNetCore.Authorization;
using MongoDB.Driver;
using Persistence;

namespace Infrastructure.Security;

public class IsAdministrator : IAuthorizationRequirement
{
}

public class IsAdministratorHandler : AuthorizationHandler<IsAdministrator>
{
  private readonly IMongoCollection<AppUser> _collection;

  public IsAdministratorHandler(MongoDbContext context)
  {
    _collection = context.GetCollection<AppUser>("users");
  }

  protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, IsAdministrator requirement)
  {
    var userEmail = context.User.FindFirstValue(ClaimTypes.Email);

    if (userEmail == null) return Task.CompletedTask;

    var user = _collection.Find(x => x.Email == userEmail).SingleOrDefaultAsync().Result;

    if (user == null) return Task.CompletedTask;

    if (user.Role == "Admin") context.Succeed(requirement);

    return Task.CompletedTask;
  }
}
