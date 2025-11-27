using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.LeaveTypes;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveTypesController : ControllerBase
    {
        private readonly Db _db;

        public LeaveTypesController(Db db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveTypeReadDto>>> GetAll()
        {
            using IDbConnection conn = _db.CreateConnection();

            var rows = await conn.QueryAsync<LeaveTypeReadDto>(
                "SELECT LeaveTypeID, TypeName FROM LeaveTypes ORDER BY LeaveTypeID");

            return Ok(rows);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LeaveTypeReadDto>> GetById(int id)
        {
            using var conn = _db.CreateConnection();

            var type = await conn.QueryFirstOrDefaultAsync<LeaveTypeReadDto>(
                "SELECT LeaveTypeID, TypeName FROM LeaveTypes WHERE LeaveTypeID=@id", new { id });

            return type is null ? NotFound(new { message = "نوع الإجازة غير موجود." }) : Ok(type);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveTypeCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.TypeName))
                return BadRequest(new { message = "اسم النوع مطلوب." });

            using var conn = _db.CreateConnection();

            var duplicate = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM LeaveTypes WHERE LOWER(TypeName)=LOWER(@TypeName)",
                new { dto.TypeName });

            if (duplicate > 0)
                return Conflict(new { message = "النوع موجود مسبقاً." });

            var id = await conn.ExecuteScalarAsync<int>(@"
                INSERT INTO LeaveTypes (TypeName)
                VALUES (@TypeName);
                SELECT CAST(SCOPE_IDENTITY() AS INT);", dto);

            return CreatedAtAction(nameof(GetById), new { id }, new { LeaveTypeID = id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, LeaveTypeUpdateDto dto)
        {
            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM LeaveTypes WHERE LeaveTypeID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "نوع الإجازة غير موجود." });

            var duplicate = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM LeaveTypes WHERE LOWER(TypeName)=LOWER(@TypeName) AND LeaveTypeID<>@id",
                new { dto.TypeName, id });

            if (duplicate > 0)
                return Conflict(new { message = "اسم النوع مستخدم." });

            await conn.ExecuteAsync(@"
                UPDATE LeaveTypes
                SET TypeName=@TypeName
                WHERE LeaveTypeID=@id", new { id, dto.TypeName });

            return Ok(new { message = "تم التحديث." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = _db.CreateConnection();

            var inUse = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM LeaveRequests WHERE LeaveTypeID=@id", new { id });

            if (inUse > 0)
                return Conflict(new { message = "لا يمكن حذف النوع لوجود طلبات مرتبطة." });

            var rows = await conn.ExecuteAsync("DELETE FROM LeaveTypes WHERE LeaveTypeID=@id", new { id });

            return rows == 0 ? NotFound(new { message = "نوع الإجازة غير موجود." })
                             : Ok(new { message = "تم الحذف." });
        }
    }
}

