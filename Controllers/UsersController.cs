using BCrypt.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication3.Helpers;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.Users;
using WebApplication3.Repositories.UserRepository;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserRepository _repo;
    private readonly IConfiguration _config;

    public UsersController(IUserRepository repo, IConfiguration config)
    {
        _repo = repo;
        _config = config;
    }

    // ========== REGISTER (SuperAdmin Only) ==========
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(UserRegisterDto dto)
    {
        var existing = await _repo.GetByUsername(dto.Username);
        if (existing != null)
            return BadRequest("Username already exists");

        var user = new User
        {
            Username = dto.Username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
            Role = dto.Role, // 0,1,2,3
            EmployeeID = dto.EmployeeID,
            DepartmentID = dto.DepartmentID,
            IsActive = true,
            CreatedAt = DateTime.Now
        };

        await _repo.Create(user);
        return Ok("User Registered");
    }

    // ========== LOGIN (Everyone) ==========
    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserLoginDto dto)
    {
        var user = await _repo.GetByUsername(dto.Username);

        if (user == null)
            return Unauthorized("User not found");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            return Unauthorized("Invalid password");

        if (!user.IsActive)
            return Unauthorized("User is disabled");

        var token = GenerateJwtToken(user);

        return Ok(new
        {
            Token = token,
            UserID = user.UserID,
            Role = user.Role,
            EmployeeID = user.EmployeeID,
            DepartmentID = user.DepartmentID
        });
    }

    // ========== GENERATE JWT TOKEN ==========
    private string GenerateJwtToken(User user)
    {
        var key = Encoding.UTF8.GetBytes(_config["Jwt:Key"]);

        var claims = new List<Claim>
        {
            new Claim("UserID", user.UserID.ToString()),
            new Claim("Role", user.Role.ToString()),
            new Claim("EmployeeID", user.EmployeeID?.ToString() ?? ""),
            new Claim("DepartmentID", user.DepartmentID?.ToString() ?? "")
        };

        var creds = new SigningCredentials(
            new SymmetricSecurityKey(key),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(10),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // ========== GET ALL USERS (SuperAdmin Only) ==========
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }

    // ========== GET USER BY ID (SuperAdmin Only) ==========
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var user = await _repo.GetById(id);
        return user == null ? NotFound() : Ok(user);
    }

    // ========== DELETE USER (SuperAdmin Only) ==========
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        bool result = await _repo.Delete(id);
        return result ? Ok("User Deleted") : NotFound("User not found");
    }
}
