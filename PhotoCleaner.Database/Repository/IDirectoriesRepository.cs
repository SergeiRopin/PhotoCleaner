using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoCleaner.Database.Repository
{
    public interface IDirectoriesRepository : IRepository
    {
        Task<IEnumerable<string>> GetAllPagedAsync(int number, string dirType);
        Task CreateOrUpdateAsync(string directory, string dirType);
    }
}
