using MongoDB.Driver;
using PhotoCleaner.Database.Dto;
using PhotoCleaner.Database.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoCleaner.Database.MongoDB.Repository.FileExtensioins
{
    public class MongoFileExtensionsRepository : IFileExtensionsRepository
    {
        public string CollectionName => "fileExtensions";

        public async Task<IEnumerable<FileExtensionDto>> GetAllAsync(string dirType)
        {
            var collection = MongoDbConnector.GetConnection().GetCollection<FileExtensionDto>(CollectionName);
            //TODO: Remove "Source" use appropriate model from PhotoCleaner.Core
            var filter = string.Equals(dirType, "Source", StringComparison.InvariantCultureIgnoreCase) 
                ? Builders<FileExtensionDto>.Filter.Eq(x => x.IsSource, true)
                : Builders<FileExtensionDto>.Filter.Eq(x => x.IsTarget, true);
            return await collection.Find(filter).ToListAsync();
        }

        public void MakeFavourite(string dirType, string extension)
        {
            throw new NotImplementedException();
        }
    }
}
