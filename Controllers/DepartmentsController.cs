using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.Departments;

namespace WebApplication2.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentsController : ControllerBase
    {
        private readonly Db _db;

        public DepartmentsController(Db db)
        {
            _db = db;
        }

        // GET: api/departments
        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentReadDto>>> GetAll(string? search)
        {
            using IDbConnection conn = _db.CreateConnection();

            string sql = @"
                SELECT DepartmentID, DepartmentName
                FROM Departments
                WHERE (@search IS NULL OR DepartmentName LIKE '%' + @search + '%')
                ORDER BY DepartmentID
            ";

            var data = await conn.QueryAsync<DepartmentReadDto>(sql, new { search });

            return Ok(data);
        }

        // GET: api/departments/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<DepartmentReadDto>> GetById(int id)
        {
            using var conn = _db.CreateConnection();

            var dep = await conn.QueryFirstOrDefaultAsync<DepartmentReadDto>(
                "SELECT DepartmentID, DepartmentName FROM Departments WHERE DepartmentID=@id",
                new { id });

            return dep == null ? NotFound(new { message = "القسم غير موجود." }) : Ok(dep);
        }

        // POST: api/departments
        [HttpPost]
        public async Task<ActionResult> Create(DepartmentCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DepartmentName))
                return BadRequest(new { message = "اسم القسم مطلوب." });

            using var conn = _db.CreateConnection();

            // تحقق من التكرار
            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Departments WHERE LOWER(DepartmentName)=LOWER(@name)",
                new { name = dto.DepartmentName });

            if (exists > 0)
                return Conflict(new { message = "اسم القسم موجود مسبقاً." });

            var id = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO Departments (DepartmentName)
                VALUES (@DepartmentName);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ", dto);

            return CreatedAtAction(nameof(GetById), new { id }, new { DepartmentID = id, dto.DepartmentName });
        }

        // PUT: api/departments/5
        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, DepartmentUpdateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DepartmentName))
                return BadRequest(new { message = "اسم القسم مطلوب." });

            using var conn = _db.CreateConnection();

            // تحقق من وجود القسم
            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Departments WHERE DepartmentID=@id",
                new { id });

            if (exists == 0)
                return NotFound(new { message = "القسم غير موجود." });

            // تحقق من تكرار الاسم لقسم آخر
            var nameTaken = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Departments WHERE LOWER(DepartmentName)=LOWER(@name) AND DepartmentID<>@id",
                new { name = dto.DepartmentName, id });

            if (nameTaken > 0)
                return Conflict(new { message = "الاسم مستخدم لقسم آخر." });

            await conn.ExecuteAsync(@"
                UPDATE Departments
                SET DepartmentName = @DepartmentName
                WHERE DepartmentID=@id
            ", new { dto.DepartmentName, id });

            return Ok(new { message = "تم التحديث." });
        }

        // DELETE: api/departments/5
        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Departments WHERE DepartmentID=@id",
                new { id });

            if (exists == 0)
                return NotFound(new { message = "القسم غير موجود." });

            // هل يوجد موظفين مرتبطين؟
            var hasEmployees = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Employees WHERE DepartmentID=@id",
                new { id });

            if (hasEmployees > 0)
                return Conflict(new { message = "لا يمكن الحذف لوجود موظفين مرتبطين بهذا القسم." });

            await conn.ExecuteAsync("DELETE FROM Departments WHERE DepartmentID=@id", new { id });

            return Ok(new { message = "تم الحذف." });
        }
    }
}
