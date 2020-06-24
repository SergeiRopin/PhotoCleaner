using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PhotoCleaner.Database.Dto
{
    public class FavouriteExtensionDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string ExtensionId { get; set; }
        public string FileType { get; set; }
    }
}
