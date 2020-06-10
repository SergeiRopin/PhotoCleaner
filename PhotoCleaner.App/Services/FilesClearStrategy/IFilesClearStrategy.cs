using PhotoCleaner.App.Domain;
using System.Threading.Tasks;

namespace PhotoCleaner.App.Services.FilesClearStrategy
{
    public interface IFilesClearStrategy
    {
        Task<ClearOperationResult> ClearFiles();
    }
}
