using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
namespace Domain;

public class AppUser
{
  [BsonId]
  [BsonRepresentation(BsonType.ObjectId)]
  public string Id { get; set; }
  public string Username { get; set; }
  public string Email { get; set; }
  public string Password { get; set; }
  public string Role { get; set; }
  /*public string Role
  {
    get { return Role; }
    set
    {
      if (string.IsNullOrEmpty(value) || value != "Admin")
      {
        Role = "Standard";
      }
      else
      {
        Role = value;
      }
    }
  }*/
}
