using Application.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Image;

public class StorageAccessor : IStorageAccessor
{
  private readonly BlobContainerClient _blobContainerClient;

  public StorageAccessor(BlobServiceClient blobServiceClient, IConfiguration config)
  {
    var containerName = config["Storage:ContainerName"];

    _blobContainerClient = blobServiceClient.GetBlobContainerClient(containerName);
  }

  public async Task<string> AddImage(IFormFile file)
  {
    if (file.Length > 0)
    {
      if (!_blobContainerClient.Exists()) return null;

      var fileName = $"{Guid.NewGuid()}_{file.FileName}";

      var blobClient = _blobContainerClient.GetBlobClient(fileName);

      using var stream = file.OpenReadStream();
      await blobClient.UploadAsync(stream, true);

      return blobClient.Uri.AbsoluteUri;
    }

    return null;
  }

  public async Task DeleteImage(string filename)
  {
    if (!_blobContainerClient.Exists()) return;

    var blobClient = _blobContainerClient.GetBlobClient(filename);

    await blobClient.DeleteIfExistsAsync();

    return;
  }
}
