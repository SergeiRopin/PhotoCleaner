using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PhotoCleaner.Database.Dto
{
    public class FileExtensionDto
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
        public string Filter { get; set; }
        public string SearchPattern { get; set; }
        public bool IsSource { get; set; }
        public bool IsTarget { get; set; }
    }
}
