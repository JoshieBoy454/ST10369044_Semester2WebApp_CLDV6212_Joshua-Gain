using Azure.Storage.Files.Shares;
using Azure.Storage.Files.Shares.Models;

namespace ST10369044Semerster2WebApp.Services
{
    public class FileService
    {
        private readonly ShareServiceClient _shareServiceClient;
        public FileService(IConfiguration configuration)
        {
            _shareServiceClient = new ShareServiceClient(configuration["AzureStorage:ConnectionString"]);
        }

        public async Task UploadFileAsync(string shareName, string fileName, Stream content)
        {
            var shareClient = _shareServiceClient.GetShareClient(shareName);
            await shareClient.CreateIfNotExistsAsync();
            var directoryClient = shareClient.GetRootDirectoryClient();
            var fileClient = directoryClient.GetFileClient(fileName);
            await fileClient.CreateAsync(content.Length);
            await fileClient.UploadAsync(content);
        }
    }
}

