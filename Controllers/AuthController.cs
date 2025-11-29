using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Dapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Helpers;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly Db _db;
        private readonly IConfiguration _config;

        public AuthController(Db db, IConfiguration config)
        {
            _db = db;
            _config = config;
        }

        // ===== LOGIN =====
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(AuthLoginDto dto)
        {
            using IDbConnection conn = _db.CreateConnection();

            var user = await conn.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Username = @Username AND IsActive = 1",
                new { dto.Username });

            if (user == null || !PasswordHasher.VerifyPassword(dto.Password, user.PasswordHash))
                return Unauthorized(new { message = "اسم المستخدم أو كلمة المرور غير صحيحة." });

            var token = GenerateJwtToken(user, out DateTime expiresAt);

            var result = new AuthResultDto
            {
                Token = token,
                ExpiresAt = expiresAt,
                Username = user.Username,
                Role = user.Role,
                EmployeeID = user.EmployeeID,
                DepartmentID = user.DepartmentID
            };

            return Ok(result);
        }
        [HttpGet("test-hash")]
        public IActionResult TestHash()
        {
            string pwd = "123";
            string hash = PasswordHasher.HashPassword(pwd);
            return Ok(new { pwd, hash });
        }


        // ===== REGISTER (فقط للسوبر أدمن) =====
        [HttpPost("register")]
        [Authorize(Roles = "SuperAdmin")]
        public async Task<IActionResult> Register(AuthRegisterDto dto)
        {
            using IDbConnection conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Users WHERE Username = @Username",
                new { dto.Username });

            if (exists > 0)
                return Conflict(new { message = "اسم المستخدم موجود مسبقاً." });

            var passwordHash = PasswordHasher.HashPassword(dto.Password);

            var sql = @"
                INSERT INTO Users (Username, PasswordHash, Role, EmployeeID, DepartmentID, IsActive)
                VALUES (@Username, @PasswordHash, @Role, @EmployeeID, @DepartmentID, 1);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await conn.ExecuteScalarAsync<int>(sql, new
            {
                dto.Username,
                PasswordHash = passwordHash,
                dto.Role,
                dto.EmployeeID,
                dto.DepartmentID
            });

            return Ok(new { message = "تم إنشاء المستخدم بنجاح.", UserID = id });
        }

        private string GenerateJwtToken(User user, out DateTime expiresAt)
        {
            var jwtSection = _config.GetSection("Jwt");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection["Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserID.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            if (user.EmployeeID.HasValue)
                claims.Add(new Claim("EmployeeID", user.EmployeeID.Value.ToString()));
            if (user.DepartmentID.HasValue)
                claims.Add(new Claim("DepartmentID", user.DepartmentID.Value.ToString()));

            int expireMinutes = int.TryParse(jwtSection["ExpireMinutes"], out var m) ? m : 30;
            expiresAt = DateTime.UtcNow.AddMinutes(expireMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtSection["Issuer"],
                audience: jwtSection["Audience"],
                claims: claims,
                expires: expiresAt,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
