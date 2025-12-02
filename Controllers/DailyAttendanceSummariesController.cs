using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.DailyAttendanceSummaries;
using WebApplication3.Repositories.DailyAttendanceSummaryRepository;

[ApiController]
[Route("api/[controller]")]
public class DailyAttendanceSummaryController : ControllerBase
{
    private readonly IDailyAttendanceSummaryRepository _repo;

    public DailyAttendanceSummaryController(IDailyAttendanceSummaryRepository repo)
    {
        _repo = repo;
    }

    // ================================
    // Helpers (Role / Employee / Dept)
    // ================================
    private string? GetRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }

    private int? GetEmployeeId() =>
        int.TryParse(User.FindFirstValue("EmployeeID"), out var id) ? id : null;

    private int? GetDeptId() =>
        int.TryParse(User.FindFirstValue("DepartmentID"), out var id) ? id : null;


    // ============================================
    // SUPERADMIN + ADMIN: Get All Summaries
    // UNITADMIN: Only his department
    // EMPLOYEE: Only his own
    // ============================================
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var role = GetRole();

        if (role == "SuperAdmin" || role == "Admin")
            return Ok(await _repo.GetAll());

        if (role == "UnitAdmin")
            return Ok(await _repo.GetByDepartment(GetDeptId()!.Value));

        if (role == "Employee")
            return Ok(await _repo.GetByEmployee(GetEmployeeId()!.Value));

        return Unauthorized();
    }


    // ============================================
    // GET ONE SUMMARY
    // ============================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(long id)
    {
        var item = await _repo.GetById(id);
        if (item == null) return NotFound();

        var role = GetRole();

        if (role == "SuperAdmin" || role == "Admin")
            return Ok(item);

        if (role == "UnitAdmin")
        {
            if (item.EmployeeDeptID == GetDeptId())
                return Ok(item);

            return Unauthorized("Not your department");
        }

        if (role == "Employee")
        {
            if (item.EmployeeID == GetEmployeeId())
                return Ok(item);

            return Unauthorized("Not your data");
        }

        return Unauthorized();
    }


    // ============================================
    // CREATE SUMMARY
    // Only System/Admin or Scheduler should create
    // ============================================
    [HttpPost]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Create(DailyAttendanceSummaryCreateDto dto)
    {
        var model = new DailyAttendanceSummary
        {
            EmployeeID = dto.EmployeeID,
            WorkDate = dto.WorkDate,
            Status = dto.Status,
            TotalWorkMinutes = dto.TotalWorkMinutes,
            LateMinutes = dto.LateMinutes,
            OvertimeMinutes = dto.OvertimeMinutes
        };

        return (await _repo.Create(model)) > 0
            ? Ok("Daily summary created")
            : BadRequest();
    }


    // ============================================
    // UPDATE SUMMARY
    // ============================================
    [HttpPut]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(DailyAttendanceSummaryUpdateDto dto)
    {
        var model = await _repo.GetById(dto.SummaryID);
        if (model == null) return NotFound();

        model.EmployeeID = dto.EmployeeID;
        model.WorkDate = dto.WorkDate;
        model.Status = dto.Status;
        model.TotalWorkMinutes = dto.TotalWorkMinutes;
        model.LateMinutes = dto.LateMinutes;
        model.OvertimeMinutes = dto.OvertimeMinutes;

        return (await _repo.Update(model)) > 0
            ? Ok("Daily summary updated")
            : BadRequest();
    }


    // ============================================
    // DELETE SUMMARY
    // ============================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(long id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("Daily summary deleted")
            : NotFound();
    }
}
