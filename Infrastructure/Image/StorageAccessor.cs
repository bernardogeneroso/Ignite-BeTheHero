using Application.Interfaces;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Image;

public class StorageAccessor : IStorageAccessor
{
  private readonly IConfiguration _config;
  private readonly BlobServiceClient _blobServiceClient;

  public StorageAccessor(BlobServiceClient blobServiceClient, IConfiguration config)
  {
    _blobServiceClient = blobServiceClient;
    _config = config;
  }

  public async Task<string> AddImage(IFormFile file)
  {
    if (file.Length > 0)
    {
      var containerName = _config["Storage:ContainerName"];

      var containerClient = _blobServiceClient.GetBlobContainerClient(containerName);
      var blobClient = containerClient.GetBlobClient(file.FileName);

      using var stream = file.OpenReadStream();
      await blobClient.UploadAsync(stream, true);

      return blobClient.Uri.ToString();
    }

    return null;
  }

  public Task<string> DeleteImage(string publicId)
  {
    throw new NotImplementedException();
  }
}
