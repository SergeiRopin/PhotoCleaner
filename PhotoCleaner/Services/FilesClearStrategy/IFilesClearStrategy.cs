using PhotoCleaner.Domain;
using System.Threading.Tasks;

namespace PhotoCleaner.Services.FilesClearStrategy
{
    public interface IFilesClearStrategy
    {
        Task<ClearOperationResult> ClearFiles();
    }
}
