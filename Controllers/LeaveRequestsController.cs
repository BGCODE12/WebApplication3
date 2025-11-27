using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.LeaveRequests;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly Db _db;

        public LeaveRequestsController(Db db)
        {
            _db = db;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestReadDto>>> GetAll([FromQuery] int? employeeId, [FromQuery] string? status)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = @"
                SELECT LeaveRequestID, EmployeeID, LeaveTypeID, StartDate, EndDate, Status, ApprovedByEmployeeID
                FROM LeaveRequests
                WHERE (@employeeId IS NULL OR EmployeeID=@employeeId)
                  AND (@status IS NULL OR Status=@status)
                ORDER BY StartDate DESC, LeaveRequestID DESC";

            var rows = await conn.QueryAsync<LeaveRequestReadDto>(sql, new { employeeId, status });
            return Ok(rows);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<LeaveRequestReadDto>> GetById(int id)
        {
            using var conn = _db.CreateConnection();

            var row = await conn.QueryFirstOrDefaultAsync<LeaveRequestReadDto>(
                @"SELECT LeaveRequestID, EmployeeID, LeaveTypeID, StartDate, EndDate, Status, ApprovedByEmployeeID
                  FROM LeaveRequests WHERE LeaveRequestID=@id", new { id });

            return row is null ? NotFound(new { message = "طلب الإجازة غير موجود." }) : Ok(row);
        }

        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequestCreateDto dto)
        {
            if (dto.StartDate > dto.EndDate)
                return BadRequest(new { message = "تاريخ البدء يجب أن يكون قبل تاريخ النهاية." });

            using var conn = _db.CreateConnection();

            var sql = @"
                INSERT INTO LeaveRequests (EmployeeID, LeaveTypeID, StartDate, EndDate, Status, ApprovedByEmployeeID)
                VALUES (@EmployeeID, @LeaveTypeID, @StartDate, @EndDate, @Status, @ApprovedByEmployeeID);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await conn.ExecuteScalarAsync<int>(sql, dto);

            return CreatedAtAction(nameof(GetById), new { id }, new { LeaveRequestID = id });
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, LeaveRequestUpdateDto dto)
        {
            if (dto.StartDate > dto.EndDate)
                return BadRequest(new { message = "تاريخ البدء يجب أن يكون قبل تاريخ النهاية." });

            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM LeaveRequests WHERE LeaveRequestID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "طلب الإجازة غير موجود." });

            var sql = @"
                UPDATE LeaveRequests
                SET LeaveTypeID=@LeaveTypeID,
                    StartDate=@StartDate,
                    EndDate=@EndDate,
                    Status=@Status,
                    ApprovedByEmployeeID=@ApprovedByEmployeeID
                WHERE LeaveRequestID=@id";

            await conn.ExecuteAsync(sql, new
            {
                id,
                dto.LeaveTypeID,
                dto.StartDate,
                dto.EndDate,
                dto.Status,
                dto.ApprovedByEmployeeID
            });

            return Ok(new { message = "تم التحديث." });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync("DELETE FROM LeaveRequests WHERE LeaveRequestID=@id", new { id });

            return rows == 0 ? NotFound(new { message = "طلب الإجازة غير موجود." })
                             : Ok(new { message = "تم الحذف." });
        }
    }
}

