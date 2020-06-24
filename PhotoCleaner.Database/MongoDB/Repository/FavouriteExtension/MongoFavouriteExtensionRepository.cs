using MongoDB.Driver;
using PhotoCleaner.Database.Dto;
using PhotoCleaner.Database.Repository;
using System.Threading.Tasks;

namespace PhotoCleaner.Database.MongoDB.Repository.FavouriteExtension
{
    public class MongoFavouriteExtensionRepository : IFavouriteExtensionRepository
    {
        public string CollectionName => "favouriteExtensions";

        public async Task<FavouriteExtensionDto> GetByType(string fileType)
        {
            var collection = MongoDbConnector.GetConnection().GetCollection<FavouriteExtensionDto>(CollectionName);
            var filter = Builders<FavouriteExtensionDto>.Filter.Eq(x => x.FileType, fileType);
            return await collection.Find(filter).FirstOrDefaultAsync();
        }

        public async Task CreateOrUpdate(string extensionId, string fileType)
        {
            var collection = MongoDbConnector.GetConnection().GetCollection<FavouriteExtensionDto>(CollectionName);
            var filter = Builders<FavouriteExtensionDto>.Filter.Eq(x => x.FileType, fileType);
            var update = Builders<FavouriteExtensionDto>.Update.Set(x => x.ExtensionId, extensionId);
            var updateResult = await collection.UpdateOneAsync(filter, update);

            if (updateResult.ModifiedCount == 0)
            {
                var favExtension = new FavouriteExtensionDto
                {
                    ExtensionId = extensionId,
                    FileType = fileType
                };

                await collection.InsertOneAsync(favExtension);
            }
        }

        public async Task Delete(string fileType)
        {
            var collection = MongoDbConnector.GetConnection().GetCollection<FavouriteExtensionDto>(CollectionName);
            var filter = Builders<FavouriteExtensionDto>.Filter.Eq(x => x.FileType, fileType);
            await collection.DeleteOneAsync(filter);
        }
    }
}
