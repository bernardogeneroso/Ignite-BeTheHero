using API.DTOs;
using API.Services;
using Application.Interfaces;
using Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Persistence;

namespace API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
  private readonly TokenService _tokenService;
  private readonly IMongoCollection<AppUser> _collection;
  private readonly IStorageAccessor _storageAccessor;

  public AccountController(TokenService TokenService, MongoDbContext context, IStorageAccessor storageAccessor)
  {
    _storageAccessor = storageAccessor;
    _tokenService = TokenService;
    _collection = context.GetCollection<AppUser>("users");
  }

  [HttpGet]
  public IActionResult GetUser()
  {
    return Ok();
  }

  [AllowAnonymous]
  [HttpPost("login")]
  public async Task<ActionResult<UserDto>> Login(LoginDto LoginDto)
  {
    var user = await _collection.Find(x => x.Email == LoginDto.Email).FirstOrDefaultAsync();

    if (user == null) return BadRequest("Invalid username or password");

    var verifyPassword = BCrypt.Net.BCrypt.Verify(LoginDto.Password, user.Password);

    if (!verifyPassword) return BadRequest("Invalid username or password");

    var token = _tokenService.CreateToken(user);

    return Ok(new UserDto
    {
      Username = user.Username,
      Email = user.Email,
      Role = user.Role,
      Token = token
    });
  }

  [AllowAnonymous]
  [HttpPost("register")]
  public async Task<IActionResult> Register(RegisterDto RegisterDto)
  {
    var userExist = await _collection.FindAsync(x => x.Email == RegisterDto.Email);

    if (userExist.Any()) return BadRequest("This email is already in use.");

    var password = BCrypt.Net.BCrypt.HashPassword(RegisterDto.Password);

    var newUser = new AppUser
    {
      Username = RegisterDto.Username,
      Email = RegisterDto.Email,
      Password = password,
      Role = "Standard"
    };

    await _collection.InsertOneAsync(newUser);

    return Ok();
  }

  [HttpPost("avatar")]
  public async Task<IActionResult> AddImage([FromForm] IFormFile File)
  {
    var avatarUrl = await _storageAccessor.AddImage(File);

    if (avatarUrl == null) return BadRequest("Could not upload image");

    return Ok(avatarUrl);
  }

  [HttpDelete("avatar/{filename}")]
  public async Task<IActionResult> DeleteImage(string filename)
  {
    await _storageAccessor.DeleteImage(filename);

    return NoContent();
  }
}
