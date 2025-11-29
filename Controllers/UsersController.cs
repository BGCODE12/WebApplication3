using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Helpers;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.Auth;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")] // 🔐 السوبر أدمن فقط
    public class UsersController : ControllerBase
    {
        private readonly Db _db;

        public UsersController(Db db)
        {
            _db = db;
        }

        // ===================== 1) عرض جميع المستخدمين =====================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using IDbConnection conn = _db.CreateConnection();
            var sql = "SELECT * FROM Users";
            var users = await conn.QueryAsync<User>(sql);
            return Ok(users);
        }

        // ===================== 2) عرض مستخدم واحد =====================
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            using IDbConnection conn = _db.CreateConnection();
            var user = await conn.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE UserID = @id", new { id });

            return user is null ? NotFound() : Ok(user);
        }

        // ===================== 3) إنشاء مستخدم جديد =====================
        [HttpPost("create")]
        public async Task<IActionResult> Create(AuthRegisterDto dto)
        {
            using IDbConnection conn = _db.CreateConnection();

            // تحقق من عدم تكرار الاسم
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

        // ===================== 4) تعديل مستخدم =====================
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, AuthRegisterDto dto)
        {
            using IDbConnection conn = _db.CreateConnection();

            var user = await conn.QueryFirstOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE UserID = @id", new { id });

            if (user == null)
                return NotFound(new { message = "المستخدم غير موجود." });

            string newPasswordHash = user.PasswordHash;

            if (!string.IsNullOrWhiteSpace(dto.Password))
                newPasswordHash = PasswordHasher.HashPassword(dto.Password);

            var sql = @"
                UPDATE Users SET
                    Username = @Username,
                    PasswordHash = @PasswordHash,
                    Role = @Role,
                    EmployeeID = @EmployeeID,
                    DepartmentID = @DepartmentID
                WHERE UserID = @id";

            await conn.ExecuteAsync(sql, new
            {
                id,
                dto.Username,
                PasswordHash = newPasswordHash,
                dto.Role,
                dto.EmployeeID,
                dto.DepartmentID
            });

            return Ok(new { message = "تم تحديث المستخدم بنجاح." });
        }

        // ===================== 5) تعطيل المستخدم =====================
        [HttpPut("{id:int}/disable")]
        public async Task<IActionResult> Disable(int id)
        {
            using IDbConnection conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync(
                "UPDATE Users SET IsActive = 0 WHERE UserID = @id",
                new { id });

            if (rows == 0)
                return NotFound(new { message = "المستخدم غير موجود." });

            return Ok(new { message = "تم تعطيل المستخدم." });
        }

        // ===================== 6) تفعيل المستخدم =====================
        [HttpPut("{id:int}/enable")]
        public async Task<IActionResult> Enable(int id)
        {
            using IDbConnection conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync(
                "UPDATE Users SET IsActive = 1 WHERE UserID = @id",
                new { id });

            if (rows == 0)
                return NotFound(new { message = "المستخدم غير موجود." });

            return Ok(new { message = "تم تفعيل المستخدم." });
        }

        // ===================== 7) نقل مستخدم إلى قسم آخر =====================
        [HttpPut("{id:int}/move-department/{newDept:int}")]
        public async Task<IActionResult> MoveToDepartment(int id, int newDept)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = "UPDATE Users SET DepartmentID = @newDept WHERE UserID = @id";
            var rows = await conn.ExecuteAsync(sql, new { id, newDept });

            if (rows == 0)
                return NotFound(new { message = "المستخدم غير موجود." });

            return Ok(new { message = "تم نقل المستخدم إلى قسم جديد." });
        }

        // ===================== 8) تغيير EmployeeID للمستخدم =====================
        [HttpPut("{id:int}/assign-employee/{employeeId:int}")]
        public async Task<IActionResult> AssignEmployee(int id, int employeeId)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = "UPDATE Users SET EmployeeID = @employeeId WHERE UserID = @id";
            var rows = await conn.ExecuteAsync(sql, new { id, employeeId });

            if (rows == 0)
                return NotFound(new { message = "المستخدم غير موجود." });

            return Ok(new { message = "تم ربط المستخدم بالموظف." });
        }

        // ===================== 9) حذف المستخدم =====================
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using IDbConnection conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync(
                "DELETE FROM Users WHERE UserID = @id",
                new { id });

            if (rows == 0)
                return NotFound(new { message = "المستخدم غير موجود." });

            return Ok(new { message = "تم حذف المستخدم بنجاح." });
        }
        [HttpGet("test-hash")]
        public IActionResult TestHash()
        {
            string pwd = "123";
            string hash = PasswordHasher.HashPassword(pwd);
            return Ok(new { pwd, hash });
        }

    }
}
