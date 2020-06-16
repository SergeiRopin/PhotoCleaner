using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoCleaner.Database.Repository
{
    public interface IDirectoriesRepository
    {
        Task<IEnumerable<string>> GetAllPagedAsync(byte number, string dirType);
        Task CreateOrUpdateAsync(string directory, string dirType);
    }
}
