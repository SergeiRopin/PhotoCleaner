using PhotoCleaner.Database.Dto;
using PhotoCleaner.Database.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PhotoCleaner.Database.Local
{
    /// <summary>Hardcoded, in-memory data. Nothing is persisted between application runs.</summary>
    internal static class LocalStore
    {
        public static readonly object Sync = new object();

        public static readonly List<FileExtensionDto> Extensions = new List<FileExtensionDto>
        {
            new FileExtensionDto { Id = "1", Name = "JPEG", Filter = "JPEG files | *.jpg", SearchPattern = "*.jpg", IsSource = true, IsTarget = true },
            new FileExtensionDto { Id = "2", Name = "RAW", Filter = "RAW files | *.NEF", SearchPattern = "*.NEF", IsSource = true, IsTarget = true }
        };

        public static readonly List<DirectoryDto> Directories = new List<DirectoryDto>();
        public static readonly List<FavouriteExtensionDto> Favourites = new List<FavouriteExtensionDto>();
    }

    public class LocalFileExtensionsRepository : IFileExtensionsRepository
    {
        public string CollectionName => "fileExtensions";

        public Task<IEnumerable<FileExtensionDto>> GetAllAsync(string fileType)
        {
            bool source = string.Equals(fileType, "Source", StringComparison.InvariantCultureIgnoreCase);
            lock (LocalStore.Sync)
            {
                return Task.FromResult<IEnumerable<FileExtensionDto>>(
                    LocalStore.Extensions.Where(x => source ? x.IsSource : x.IsTarget).ToList());
            }
        }
    }

    public class LocalDirectoriesRepository : IDirectoriesRepository
    {
        public string CollectionName => "directories";

        public Task<IEnumerable<string>> GetAllPagedAsync(int number, string dirType)
        {
            lock (LocalStore.Sync)
            {
                return Task.FromResult<IEnumerable<string>>(
                    LocalStore.Directories.Where(x => x.Type == dirType)
                        .OrderByDescending(x => x.CreatedAt)
                        .Take(number)
                        .Select(x => x.Name)
                        .ToList());
            }
        }

        public Task CreateOrUpdateAsync(string directory, string dirType)
        {
            lock (LocalStore.Sync)
            {
                var existing = LocalStore.Directories.FirstOrDefault(x => x.Type == dirType && x.Name == directory);
                if (existing != null)
                    existing.CreatedAt = DateTime.UtcNow;
                else
                    LocalStore.Directories.Add(new DirectoryDto { Id = Guid.NewGuid().ToString("N"), Name = directory, Type = dirType, CreatedAt = DateTime.UtcNow });
            }
            return Task.CompletedTask;
        }
    }

    public class LocalFavouriteExtensionRepository : IFavouriteExtensionRepository
    {
        public string CollectionName => "favouriteExtensions";

        public Task<FavouriteExtensionDto> GetByType(string fileType)
        {
            lock (LocalStore.Sync)
                return Task.FromResult(LocalStore.Favourites.FirstOrDefault(x => x.FileType == fileType));
        }

        public Task CreateOrUpdate(string extensionId, string fileType)
        {
            lock (LocalStore.Sync)
            {
                var existing = LocalStore.Favourites.FirstOrDefault(x => x.FileType == fileType);
                if (existing != null)
                    existing.ExtensionId = extensionId;
                else
                    LocalStore.Favourites.Add(new FavouriteExtensionDto { Id = Guid.NewGuid().ToString("N"), ExtensionId = extensionId, FileType = fileType });
            }
            return Task.CompletedTask;
        }

        public Task Delete(string fileType)
        {
            lock (LocalStore.Sync)
                LocalStore.Favourites.RemoveAll(x => x.FileType == fileType);
            return Task.CompletedTask;
        }
    }
}
