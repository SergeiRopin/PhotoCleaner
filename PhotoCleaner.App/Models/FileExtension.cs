using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PhotoCleaner.App.Models
{
    public class FileExtension
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Filter { get; set; }
        public string SearchPattern { get; set; }
        public bool IsFavourite { get; set; }
    }
}
