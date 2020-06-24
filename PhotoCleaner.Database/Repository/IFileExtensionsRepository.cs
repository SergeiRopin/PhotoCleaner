using PhotoCleaner.Database.Dto;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PhotoCleaner.Database.Repository
{
    public interface IFileExtensionsRepository : IRepository
    {
        Task<IEnumerable<FileExtensionDto>> GetAllAsync(string dirType);
    }
}
