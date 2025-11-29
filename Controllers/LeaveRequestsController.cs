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

        // ===================== 1) عرض كل طلبات الإجازة =====================
        // GET: api/LeaveRequests?employeeId=1&status=Pending
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LeaveRequestReadDto>>> GetAll(
            [FromQuery] int? employeeId,
            [FromQuery] string? status)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = @"
                SELECT LeaveRequestID, EmployeeID, LeaveTypeID, StartDate, EndDate, Status, ApprovedByEmployeeID
                FROM LeaveRequests
                WHERE (@employeeId IS NULL OR EmployeeID = @employeeId)
                  AND (@status IS NULL OR Status = @status)
                ORDER BY StartDate DESC, LeaveRequestID DESC";

            var rows = await conn.QueryAsync<LeaveRequestReadDto>(sql, new { employeeId, status });
            return Ok(rows);
        }

        // ===================== 2) عرض طلبات الإجازة لموظف معيّن =====================
        // GET: api/LeaveRequests/employee/5
        [HttpGet("employee/{employeeId:int}")]
        public async Task<ActionResult<IEnumerable<LeaveRequestReadDto>>> GetByEmployee(int employeeId)
        {
            using IDbConnection conn = _db.CreateConnection();

            var sql = @"
                SELECT LeaveRequestID, EmployeeID, LeaveTypeID, StartDate, EndDate, Status, ApprovedByEmployeeID
                FROM LeaveRequests
                WHERE EmployeeID = @employeeId
                ORDER BY StartDate DESC, LeaveRequestID DESC";

            var rows = await conn.QueryAsync<LeaveRequestReadDto>(sql, new { employeeId });
            return Ok(rows);
        }

        // ===================== 3) عرض طلب واحد بالتحديد =====================
        // GET: api/LeaveRequests/10
        [HttpGet("{id:int}")]
        public async Task<ActionResult<LeaveRequestReadDto>> GetById(int id)
        {
            using var conn = _db.CreateConnection();

            var row = await conn.QueryFirstOrDefaultAsync<LeaveRequestReadDto>(
                @"SELECT LeaveRequestID, EmployeeID, LeaveTypeID, StartDate, EndDate, Status, ApprovedByEmployeeID
                  FROM LeaveRequests WHERE LeaveRequestID = @id",
                new { id });

            return row is null
                ? NotFound(new { message = "طلب الإجازة غير موجود." })
                : Ok(row);
        }

        // ===================== 4) الموظف يرفع طلب إجازة =====================
        // POST: api/LeaveRequests/submit
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitLeave(LeaveRequestCreateDto dto)
        {
            if (dto.StartDate > dto.EndDate)
                return BadRequest(new { message = "تاريخ البدء يجب أن يكون قبل تاريخ النهاية." });

            using var conn = _db.CreateConnection();

            // الحالة دائماً Pending عند إنشاء الطلب
            var sql = @"
                INSERT INTO LeaveRequests (EmployeeID, LeaveTypeID, StartDate, EndDate, Status, ApprovedByEmployeeID)
                VALUES (@EmployeeID, @LeaveTypeID, @StartDate, @EndDate, 'Pending', NULL);
                SELECT CAST(SCOPE_IDENTITY() AS INT);";

            var id = await conn.ExecuteScalarAsync<int>(sql, new
            {
                dto.EmployeeID,
                dto.LeaveTypeID,
                dto.StartDate,
                dto.EndDate
            });

            return Ok(new
            {
                message = "تم رفع طلب الإجازة بنجاح وتم إرساله للإدارة.",
                LeaveRequestID = id
            });
        }

        // (اختياري) إذا حاب تبقي مسار POST العادي بدون /submit
        // POST: api/LeaveRequests
        [HttpPost]
        public async Task<IActionResult> Create(LeaveRequestCreateDto dto)
        {
            return await SubmitLeave(dto);
        }

        // ===================== 5) تعديل طلب (اختياري – للأدمن مثلاً) =====================
        // PUT: api/LeaveRequests/10
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, LeaveRequestUpdateDto dto)
        {
            if (dto.StartDate > dto.EndDate)
                return BadRequest(new { message = "تاريخ البدء يجب أن يكون قبل تاريخ النهاية." });

            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM LeaveRequests WHERE LeaveRequestID = @id",
                new { id });

            if (exists == 0)
                return NotFound(new { message = "طلب الإجازة غير موجود." });

            var sql = @"
                UPDATE LeaveRequests
                SET LeaveTypeID          = @LeaveTypeID,
                    StartDate            = @StartDate,
                    EndDate              = @EndDate,
                    Status               = @Status,
                    ApprovedByEmployeeID = @ApprovedByEmployeeID
                WHERE LeaveRequestID      = @id";

            await conn.ExecuteAsync(sql, new
            {
                id,
                dto.LeaveTypeID,
                dto.StartDate,
                dto.EndDate,
                dto.Status,
                dto.ApprovedByEmployeeID
            });

            return Ok(new { message = "تم تحديث طلب الإجازة." });
        }

        // ===================== 6) حذف طلب إجازة =====================
        // DELETE: api/LeaveRequests/10
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = _db.CreateConnection();

            var rows = await conn.ExecuteAsync(
                "DELETE FROM LeaveRequests WHERE LeaveRequestID = @id",
                new { id });

            return rows == 0
                ? NotFound(new { message = "طلب الإجازة غير موجود." })
                : Ok(new { message = "تم حذف طلب الإجازة." });
        }

        // ===================== 7) موافقة على طلب إجازة =====================
        // PUT: api/LeaveRequests/10/approve
        [HttpPut("{id:int}/approve")]
        public async Task<IActionResult> Approve(int id, [FromBody] LeaveRequestActionDto dto)
        {
            if (dto == null || dto.ApprovedByEmployeeID <= 0)
                return BadRequest(new { message = "رقم الموظف الموافق مطلوب." });

            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM LeaveRequests WHERE LeaveRequestID = @id",
                new { id });

            if (exists == 0)
                return NotFound(new { message = "طلب الإجازة غير موجود." });

            var sql = @"
                UPDATE LeaveRequests
                SET Status               = 'Approved',
                    ApprovedByEmployeeID = @ApprovedByEmployeeID
                WHERE LeaveRequestID      = @id";

            await conn.ExecuteAsync(sql, new { id, dto.ApprovedByEmployeeID });

            return Ok(new { message = "تمت الموافقة على طلب الإجازة." });
        }

        // ===================== 8) رفض طلب إجازة =====================
        // PUT: api/LeaveRequests/10/reject
        [HttpPut("{id:int}/reject")]
        public async Task<IActionResult> Reject(int id, [FromBody] LeaveRequestActionDto dto)
        {
            if (dto == null || dto.ApprovedByEmployeeID <= 0)
                return BadRequest(new { message = "رقم الموظف الرافض مطلوب." });

            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM LeaveRequests WHERE LeaveRequestID = @id",
                new { id });

            if (exists == 0)
                return NotFound(new { message = "طلب الإجازة غير موجود." });

            var sql = @"
                UPDATE LeaveRequests
                SET Status               = 'Rejected',
                    ApprovedByEmployeeID = @ApprovedByEmployeeID
                WHERE LeaveRequestID      = @id";

            await conn.ExecuteAsync(sql, new { id, dto.ApprovedByEmployeeID });

            return Ok(new { message = "تم رفض طلب الإجازة." });
        }
    }
}
