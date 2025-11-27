using Dapper;
using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Linq;
using WebApplication3.Context;
using WebApplication3.Models.DTOs.Employees;
using WebApplication3.Models.DTOs.ShiftDay;

namespace WebApplication3.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeesController : ControllerBase
    {
        private readonly Db _db;

        public EmployeesController(Db db)
        {
            _db = db;
        }

        // ===================== GET ALL =========================

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            using var conn = _db.CreateConnection();

            var sql = @"
                SELECT e.*, 
                       d.DepartmentName,
                       s.ShiftName
                FROM Employees e
                JOIN Departments d ON e.DepartmentID = d.DepartmentID
                JOIN Shifts s      ON e.ShiftID      = s.ShiftID
                ORDER BY e.EmployeeID";

            var items = await conn.QueryAsync<EmployeeReadDto>(sql);

            return Ok(items);
        }

        // ===================== GET BY ID =========================

        [HttpGet("{id:int}")]
        public async Task<IActionResult> Get(int id)
        {
            using var conn = _db.CreateConnection();

            var sql = @"
                SELECT e.*, 
                       d.DepartmentName,
                       s.ShiftName
                FROM Employees e
                JOIN Departments d ON e.DepartmentID = d.DepartmentID
                JOIN Shifts s      ON e.ShiftID      = s.ShiftID
                WHERE e.EmployeeID = @id";

            var emp = await conn.QueryFirstOrDefaultAsync<EmployeeReadDto>(sql, new { id });

            return emp is null ? NotFound(new { message = "الموظف غير موجود." }) : Ok(emp);
        }

        // ===================== FULL DETAILS =========================

        [HttpGet("full")]
        public async Task<IActionResult> GetFullDetails(
            string? search,
            DateTime? from,
            DateTime? to,
            int page = 1,
            int pageSize = 20)
        {
            if (page <= 0 || pageSize <= 0)
                return BadRequest(new { message = "يجب أن تكون قيم الصفحة وحجمها أكبر من الصفر." });

            if (pageSize > 100)
                pageSize = 100;

            var dateFrom = (from?.Date) ?? DateTime.Today;
            var dateTo = (to?.Date) ?? DateTime.Today;

            if (dateFrom > dateTo)
                return BadRequest(new { message = "تاريخ البداية يجب ألا يتجاوز تاريخ النهاية." });

            var skip = (page - 1) * pageSize;

            var sqlEmployees = @"
                SELECT e.EmployeeID, e.FullName, e.DeviceUserID, e.UserDeviceSN,
                       e.DepartmentID, e.ShiftID,
                       d.DepartmentName,
                       s.ShiftName, s.StartTime, s.EndTime, s.GracePeriodMinutes
                FROM Employees e
                JOIN Departments d ON e.DepartmentID = d.DepartmentID
                JOIN Shifts s      ON e.ShiftID      = s.ShiftID
                WHERE (@search IS NULL OR e.FullName LIKE '%' + @search + '%')
                ORDER BY e.EmployeeID
                OFFSET @skip ROWS FETCH NEXT @take ROWS ONLY;

                SELECT COUNT(*)
                FROM Employees e
                WHERE (@search IS NULL OR e.FullName LIKE '%' + @search + '%');
            ";

            using var conn = _db.CreateConnection();
            using var multi = await conn.QueryMultipleAsync(sqlEmployees, new { search, skip, take = pageSize });

            var employeeRows = (await multi.ReadAsync<EmployeeFullRow>()).ToList();
            var total = await multi.ReadSingleAsync<int>();

            if (employeeRows.Count == 0)
            {
                return Ok(new
                {
                    total,
                    page,
                    pageSize,
                    range = new { from = dateFrom, to = dateTo },
                    items = Array.Empty<EmployeeFullDto>()
                });
            }

            var empIds = employeeRows.Select(r => r.EmployeeID).ToArray();
            var shiftIds = employeeRows.Select(r => r.ShiftID).Distinct().ToArray();

            var shiftDays = (await conn.QueryAsync<ShiftDayDto>(
                "SELECT ShiftDayID, ShiftID, DayOfWeek, IsWorkDay FROM ShiftDays WHERE ShiftID IN @ShiftIds",
                new { ShiftIds = shiftIds })).ToList();

            var sessions = (await conn.QueryAsync<EmployeeSessionDto>(
                @"
                SELECT SessionID, EmployeeID,
                       CAST(WorkDate AS date) AS WorkDate,
                       CheckInTime,
                       CheckOutTime,
                       DurationMinutes
                FROM AttendanceSessions
                WHERE EmployeeID IN @EmpIds AND WorkDate BETWEEN @From AND @To
                ORDER BY EmployeeID, WorkDate
                ", new { EmpIds = empIds, From = dateFrom, To = dateTo })).ToList();

            var summaries = (await conn.QueryAsync<EmployeeDailySummaryDto>(
                @"
                SELECT SummaryID, EmployeeID,
                       CAST(WorkDate AS date) AS WorkDate,
                       Status, TotalWorkMinutes, LateMinutes, OvertimeMinutes
                FROM DailyAttendanceSummary
                WHERE EmployeeID IN @EmpIds AND WorkDate BETWEEN @From AND @To
                ORDER BY EmployeeID, WorkDate
                ", new { EmpIds = empIds, From = dateFrom, To = dateTo })).ToList();

            var leaves = (await conn.QueryAsync<EmployeeLeaveRequestDto>(
                @"
                SELECT lr.LeaveRequestID, lr.EmployeeID, lr.LeaveTypeID,
                       lt.TypeName AS LeaveType,
                       CAST(lr.StartDate AS date) AS StartDate,
                       CAST(lr.EndDate   AS date) AS EndDate,
                       lr.Status, lr.ApprovedByEmployeeID,
                       ap.FullName AS ApproverName
                FROM LeaveRequests lr
                JOIN LeaveTypes lt ON lr.LeaveTypeID = lt.LeaveTypeID
                LEFT JOIN Employees ap ON lr.ApprovedByEmployeeID = ap.EmployeeID
                WHERE lr.EmployeeID IN @EmpIds
                  AND NOT (lr.EndDate < @From OR lr.StartDate > @To)
                ORDER BY lr.EmployeeID, lr.LeaveRequestID DESC
                ", new { EmpIds = empIds, From = dateFrom, To = dateTo })).ToList();

            var shiftDayLookup = shiftDays.GroupBy(x => x.ShiftID).ToDictionary(g => g.Key, g => g.ToList());
            var sessionLookup = sessions.GroupBy(x => x.EmployeeID).ToDictionary(g => g.Key, g => g.ToList());
            var summaryLookup = summaries.GroupBy(x => x.EmployeeID).ToDictionary(g => g.Key, g => g.ToList());
            var leaveLookup = leaves.GroupBy(x => x.EmployeeID).ToDictionary(g => g.Key, g => g.ToList());

            var items = employeeRows.Select(r =>
            {
                var dto = new EmployeeFullDto
                {
                    EmployeeID = r.EmployeeID,
                    FullName = r.FullName,
                    DeviceUserID = r.DeviceUserID,
                    UserDeviceSN = r.UserDeviceSN,
                    Department = new EmployeeDepartmentDto
                    {
                        DepartmentID = r.DepartmentID,
                        DepartmentName = r.DepartmentName
                    },
                    Shift = new EmployeeShiftDto
                    {
                        ShiftID = r.ShiftID,
                        ShiftName = r.ShiftName,
                        StartTime = r.StartTime,
                        EndTime = r.EndTime,
                        GracePeriodMinutes = r.GracePeriodMinutes
                    }
                };

                if (shiftDayLookup.TryGetValue(r.ShiftID, out var sd))
                    dto.ShiftDays = sd;

                if (sessionLookup.TryGetValue(r.EmployeeID, out var sess))
                    dto.Sessions = sess;

                if (summaryLookup.TryGetValue(r.EmployeeID, out var sums))
                    dto.DailySummaries = sums;

                if (leaveLookup.TryGetValue(r.EmployeeID, out var lr))
                    dto.LeaveRequests = lr;

                return dto;
            }).ToList();

            return Ok(new
            {
                total,
                page,
                pageSize,
                range = new { from = dateFrom, to = dateTo },
                items
            });
        }

        // ===================== CREATE =========================

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeCreateDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.FullName))
                return BadRequest(new { message = "اسم الموظف مطلوب." });

            using var conn = _db.CreateConnection();

            var sql = @"
                INSERT INTO Employees (FullName, DeviceUserID, DepartmentID, ShiftID, UserDeviceSN)
                VALUES (@FullName, @DeviceUserID, @DepartmentID, @ShiftID, @UserDeviceSN);
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";

            var newId = await conn.ExecuteScalarAsync<int>(sql, dto);

            return CreatedAtAction(nameof(Get), new { id = newId }, new { EmployeeID = newId });
        }

        // ===================== UPDATE =========================

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Update(int id, EmployeeUpdateDto dto)
        {
            using var conn = _db.CreateConnection();

            // Check exists
            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Employees WHERE EmployeeID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "الموظف غير موجود." });

            var sql = @"
                UPDATE Employees
                SET FullName = @FullName,
                    DeviceUserID = @DeviceUserID,
                    DepartmentID = @DepartmentID,
                    ShiftID = @ShiftID,
                    UserDeviceSN = @UserDeviceSN
                WHERE EmployeeID = @id;
            ";

            await conn.ExecuteAsync(sql, new { id, dto.FullName, dto.DeviceUserID, dto.DepartmentID, dto.ShiftID, dto.UserDeviceSN });

            return Ok(new { message = "تم تحديث بيانات الموظف." });
        }

        // ===================== DELETE =========================

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            using var conn = _db.CreateConnection();

            var exists = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Employees WHERE EmployeeID=@id", new { id });

            if (exists == 0)
                return NotFound(new { message = "الموظف غير موجود." });

            // Check sessions before delete
            var hasSessions = await conn.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM AttendanceSessions WHERE EmployeeID=@id", new { id });

            if (hasSessions > 0)
                return Conflict(new { message = "لا يمكن الحذف لوجود سجلات مرتبطة." });

            await conn.ExecuteAsync("DELETE FROM Employees WHERE EmployeeID=@id", new { id });

            return Ok(new { message = "تم حذف الموظف." });
        }
        private sealed class EmployeeFullRow
        {
            public int EmployeeID { get; set; }
            public string FullName { get; set; } = string.Empty;
            public string? DeviceUserID { get; set; }
            public int? UserDeviceSN { get; set; }
            public int DepartmentID { get; set; }
            public string DepartmentName { get; set; } = string.Empty;
            public int ShiftID { get; set; }
            public string ShiftName { get; set; } = string.Empty;
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public int GracePeriodMinutes { get; set; }
        }
    }
}
