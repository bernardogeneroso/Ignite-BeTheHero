using Microsoft.AspNetCore.Http;

namespace Application.Interfaces;

public interface IStorageAccessor
{
  Task<string> AddImage(IFormFile file);
  Task DeleteImage(string filename);
}