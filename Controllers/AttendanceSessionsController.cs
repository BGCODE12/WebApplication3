using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.AttendanceSessions;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceSessionsController : ControllerBase
    {
        private readonly Db _db;

        public AttendanceSessionsController(Db db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<AttendanceSessionReadDto>>> GetAll([FromQuery] int? employeeId, [FromQuery] DateTime? workDate)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = @"
                SELECT SessionID, EmployeeID, WorkDate,
                       CheckInTime,
                       CheckOutTime,
                       DurationMinutes
                FROM AttendanceSessions
                WHERE (@employeeId IS NULL OR EmployeeID = @employeeId)
                  AND (@workDate IS NULL OR WorkDate = @workDate)
                ORDER BY WorkDate DESC, SessionID DESC";

            var items = await conn.QueryAsync<AttendanceSessionReadDto>(sql, new { employeeId, workDate });
            return Ok(items);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<AttendanceSessionReadDto>> GetById(long id)
        {
            using var conn = _db.CreateConnection();

            var item = await conn.QueryFirstOrDefaultAsync<AttendanceSessionReadDto>(
                @"SELECT SessionID, EmployeeID, WorkDate,
                         CheckInTime,
                         CheckOutTime,
                         DurationMinutes
                  FROM AttendanceSessions WHERE SessionID=@id", new { id });

            return item is null ? NotFound(new { message = "الجلسة غير موجودة." }) : Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Create(AttendanceSessionCreateDto dto)
        {
            using var conn = _db.CreateConnection();

            var sql = @"
                INSERT INTO AttendanceSessions (EmployeeID, WorkDate, CheckInTime, CheckOutTime, DurationMinutes)
                VALUES (@EmployeeID, @WorkDate, @CheckInTime, @CheckOutTime, @DurationMinutes);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await conn.ExecuteScalarAsync<long>(sql, dto);

            return CreatedAtAction(nameof(GetById), new { id }, new { SessionID = id });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, AttendanceSessionUpdateDto dto)
        {
            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM AttendanceSessions WHERE SessionID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "الجلسة غير موجودة." });

            var sql = @"
                UPDATE AttendanceSessions
                SET WorkDate=@WorkDate,
                    CheckInTime=@CheckInTime,
                    CheckOutTime=@CheckOutTime,
                    DurationMinutes=@DurationMinutes
                WHERE SessionID=@id";

            await conn.ExecuteAsync(sql, new
            {
                id,
                dto.WorkDate,
                dto.CheckInTime,
                dto.CheckOutTime,
                dto.DurationMinutes
            });

            return Ok(new { message = "تم التحديث." });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            using var conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync("DELETE FROM AttendanceSessions WHERE SessionID=@id", new { id });

            return rows == 0 ? NotFound(new { message = "الجلسة غير موجودة." })
                             : Ok(new { message = "تم الحذف." });
        }
    }
}

