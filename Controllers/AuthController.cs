using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.Login;
using WebApplication3.Repositories.UserRepository;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IUserRepository _userRepo;
        private readonly IConfiguration _config;

        public AuthController(IUserRepository userRepo, IConfiguration config)
        {
            _userRepo = userRepo;
            _config = config;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var user = await _userRepo.GetByUsername(dto.Username);

            if (user == null)
                return Unauthorized("User not found");

            if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
                return Unauthorized("Invalid password");

            if (!user.IsActive)
                return Unauthorized("User is disabled");

            var token = GenerateJwtToken(user);

            return Ok(new
            {
                token,
                userId = user.UserID,
                role = ConvertRoleToName(user.Role),
                employeeId = user.EmployeeID,
                departmentId = user.DepartmentID
            });
        }

        // ===============================
        // تحويل الرقم من DB → اسم الدور
        // ===============================
        private string ConvertRoleToName(string role)
        {
            return role switch
            {
                "0" => "SuperAdmin",
                "1" => "Admin",
                "2" => "UnitAdmin",
                "3" => "Employee",
                _ => "Employee"
            };
        }

        // ===============================
        // توليد الـ JWT Token
        // ===============================
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
    {
        new Claim("UserID", user.UserID.ToString()),
        new Claim("Role", user.Role.ToString()),
        new Claim("EmployeeID", user.EmployeeID?.ToString() ?? ""),
        new Claim("DepartmentID", user.DepartmentID?.ToString() ?? "")
    };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            int expireMinutes = int.Parse(_config["Jwt:ExpireMinutes"]);

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }
}
