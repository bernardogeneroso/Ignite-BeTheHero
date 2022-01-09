using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IStorageAccessor
{
  Task<string> AddImage(IFormFile file);
  Task<string> DeleteImage(string publicId);
}