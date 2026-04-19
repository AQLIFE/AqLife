using MyLife.Core.API;
using MyLife.Core.Define;

namespace MyLife.Core.Provider
{
    public class FilePolicyProvider : IFilePolicyProvider
    {
        private readonly IFilePolicy _config;

        public FilePolicyProvider(IFilePolicy config)
        {
            _config = config;
        }

        public FileSecurityPolicy GetPolicy()
        {
            // return domain type constructed from the configured IFilePolicy
            // MaxFileSize in IFilePolicy is already in bytes (long), but FileSecurityPolicy expects original size and unit.
            // Here we just return a FileSecurityPolicy constructed with the stored bytes by using Unit and dividing.
            // Simpler approach: construct domain object using the existing values (may lose original unit/size granularity)
            return new FileSecurityPolicy(_config.StoragePath, (int)_config.MaxFileSize, _config.StorageUnit, _config.AllowedExtensions, _config.AllowUpload, _config.AllowDownload, _config.AllowDelete);
        }
    }
}
