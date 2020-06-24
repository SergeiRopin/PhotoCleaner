using PhotoCleaner.Database.Dto;
using System.Threading.Tasks;

namespace PhotoCleaner.Database.Repository
{
    public interface IFavouriteExtensionRepository : IRepository
    {
        Task<FavouriteExtensionDto> GetByType(string fileType);
        Task CreateOrUpdate(string extensionId, string fileType);
        Task Delete(string fileType);
    }
}
