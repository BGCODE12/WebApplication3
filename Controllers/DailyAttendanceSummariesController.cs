using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.DailyAttendanceSummaries;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DailyAttendanceSummariesController : ControllerBase
    {
        private readonly Db _db;

        public DailyAttendanceSummariesController(Db db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DailyAttendanceSummaryReadDto>>> GetAll([FromQuery] int? employeeId, [FromQuery] DateTime? workDate)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = @"
                SELECT SummaryID, EmployeeID, WorkDate, Status, TotalWorkMinutes, LateMinutes, OvertimeMinutes
                FROM DailyAttendanceSummary
                WHERE (@employeeId IS NULL OR EmployeeID = @employeeId)
                  AND (@workDate IS NULL OR WorkDate = @workDate)
                ORDER BY WorkDate DESC, SummaryID DESC";

            var rows = await conn.QueryAsync<DailyAttendanceSummaryReadDto>(sql, new { employeeId, workDate });
            return Ok(rows);
        }

        [HttpGet("{id:long}")]
        public async Task<ActionResult<DailyAttendanceSummaryReadDto>> GetById(long id)
        {
            using var conn = _db.CreateConnection();

            var row = await conn.QueryFirstOrDefaultAsync<DailyAttendanceSummaryReadDto>(
                @"SELECT SummaryID, EmployeeID, WorkDate, Status, TotalWorkMinutes, LateMinutes, OvertimeMinutes
                  FROM DailyAttendanceSummary WHERE SummaryID=@id", new { id });

            return row is null ? NotFound(new { message = "السجل غير موجود." }) : Ok(row);
        }

        [HttpPost]
        public async Task<IActionResult> Create(DailyAttendanceSummaryCreateDto dto)
        {
            using var conn = _db.CreateConnection();

            var sql = @"
                INSERT INTO DailyAttendanceSummary (EmployeeID, WorkDate, Status, TotalWorkMinutes, LateMinutes, OvertimeMinutes)
                VALUES (@EmployeeID, @WorkDate, @Status, @TotalWorkMinutes, @LateMinutes, @OvertimeMinutes);
                SELECT CAST(SCOPE_IDENTITY() AS BIGINT);";

            var id = await conn.ExecuteScalarAsync<long>(sql, dto);

            return CreatedAtAction(nameof(GetById), new { id }, new { SummaryID = id });
        }

        [HttpPut("{id:long}")]
        public async Task<IActionResult> Update(long id, DailyAttendanceSummaryUpdateDto dto)
        {
            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM DailyAttendanceSummary WHERE SummaryID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "السجل غير موجود." });

            var sql = @"
                UPDATE DailyAttendanceSummary
                SET Status=@Status,
                    TotalWorkMinutes=@TotalWorkMinutes,
                    LateMinutes=@LateMinutes,
                    OvertimeMinutes=@OvertimeMinutes
                WHERE SummaryID=@id";

            await conn.ExecuteAsync(sql, new
            {
                id,
                dto.Status,
                dto.TotalWorkMinutes,
                dto.LateMinutes,
                dto.OvertimeMinutes
            });

            return Ok(new { message = "تم التحديث." });
        }

        [HttpDelete("{id:long}")]
        public async Task<IActionResult> Delete(long id)
        {
            using var conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync("DELETE FROM DailyAttendanceSummary WHERE SummaryID=@id", new { id });

            return rows == 0 ? NotFound(new { message = "السجل غير موجود." })
                             : Ok(new { message = "تم الحذف." });
        }
    }
}

