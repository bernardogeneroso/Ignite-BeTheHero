using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain;

public class Record
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; }
    public Guid RoomId { get; set; }
    public string Email { get; set; }
    public string WhatsApp { get; set; }
    public string CaseTitle { get; set; }
    public string CaseDescription { get; set; }
    public decimal CasePrice { get; set; }
}
