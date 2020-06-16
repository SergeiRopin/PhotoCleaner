using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PhotoCleaner.Database.Dto
{
    public class DirectoryDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
