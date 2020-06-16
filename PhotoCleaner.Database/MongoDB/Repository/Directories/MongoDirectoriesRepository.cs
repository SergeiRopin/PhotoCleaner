using MongoDB.Bson;
using MongoDB.Driver;
using PhotoCleaner.Database.Dto;
using PhotoCleaner.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoCleaner.Database.MongoDB.Repository.Directories
{
    public class MongoDirectoriesRepository : IDirectoriesRepository
    {
        private const string CollectionName = "directories";

        public async Task<IEnumerable<string>> GetAllPagedAsync(byte number, string dirType)
        {
            var collection = MongoDbConnector.GetConnection().GetCollection<DirectoryDto>(CollectionName);

            var filter = Builders<DirectoryDto>.Filter.Eq(x => x.Type, dirType);
            var directories = await collection.Aggregate<DirectoryDto>()
                .Match(x => x.Type == dirType)
                //.Group<DirectoryDto>(new BsonDocument 
                //{ 
                //    { "_id", "$_id" }, 
                //    { "createdAt", new BsonDocument("$first", "$createdAt") },
                //    { "name", new BsonDocument("$first", "$name") },
                //    { "type", new BsonDocument("$first", "$type") },
                //})
                .SortByDescending(x => x.CreatedAt)
                .Limit(number)
                .ToListAsync();
            return directories.Select(x => x.Name);
        }

        public async Task CreateOrUpdateAsync(string directory, string dirType)
        {
            var collection = MongoDbConnector.GetConnection().GetCollection<DirectoryDto>(CollectionName);

            var lastOpenedDirectory = new DirectoryDto
            {
                Name = directory,
                Type = dirType,
                CreatedAt = DateTime.UtcNow
            };

            var builder = Builders<DirectoryDto>.Filter;
            var filter = builder.Eq(x => x.Type, dirType) & builder.Eq(x => x.Name, directory);
            var update = Builders<DirectoryDto>.Update.Set(x => x.CreatedAt, DateTime.UtcNow);

            var result = await collection.UpdateOneAsync(filter, update);

            if (result.ModifiedCount == 0)
                await collection.InsertOneAsync(lastOpenedDirectory);
        }
    }
}
