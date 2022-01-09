using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Domain;

public class AppUser
{
  private string role = "Standard";

  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }
  public string Username { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }
  public string Role
  {
    get
    {
      return role;
    }
    set
    {
      if (role == "Admin")
      {
        role = "Admin";
      }
      else
      {
        role = "Standard";
      }
    }
  }
}
