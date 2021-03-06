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
  private readonly IUserAccessor _userAccessor;

  public AccountController(TokenService TokenService, MongoDbContext context, IStorageAccessor storageAccessor, IUserAccessor userAccessor)
  {
    _userAccessor = userAccessor;
    _storageAccessor = storageAccessor;
    _tokenService = TokenService;
    _collection = context.GetCollection<AppUser>("users");
  }

  [HttpGet]
  public async Task<ActionResult<ProfileDto>> GetUser()
  {
    var user = await _collection.Find(u => u.Email == _userAccessor.GetEmail()).FirstOrDefaultAsync();

    if (user == null)
    {
      return NotFound();
    }

    return Ok(
      new ProfileDto
      {
        Username = user.Username,
        Email = user.Email,
        AvatarUrl = user.AvatarUrl,
        AvatarName = user.AvatarName,
        Role = user.Role
      }
    );
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
    var user = await _collection.Find(x => x.Email == _userAccessor.GetEmail()).FirstOrDefaultAsync();

    if (user == null) return BadRequest("Could not upload image");

    if (user.AvatarName != null)
    {
      await _storageAccessor.DeleteImage(user.AvatarName);
    }

    var avatarUrl = await _storageAccessor.AddImage(File);

    if (avatarUrl == null) return BadRequest("Could not upload image");

    user.AvatarUrl = avatarUrl;
    user.AvatarName = avatarUrl.Split('/').Last();

    await _collection.ReplaceOneAsync(x => x.Id == user.Id, user);

    return Ok(avatarUrl);
  }

  [HttpDelete("avatar/{filename}")]
  public async Task<IActionResult> DeleteImage(string filename)
  {
    var user = await _collection.Find(x => x.Email == _userAccessor.GetEmail()).FirstOrDefaultAsync();

    if (user == null) return BadRequest("Could not delete image");

    await _storageAccessor.DeleteImage(filename);

    user.AvatarUrl = null;
    user.AvatarName = null;

    await _collection.ReplaceOneAsync(x => x.Id == user.Id, user);

    return NoContent();
  }
}
