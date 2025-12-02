using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Repositories.AttendanceLogRawRepository;

[ApiController]
[Route("api/[controller]")]
public class AttendanceLogRawController : ControllerBase
{
    private readonly IAttendanceLogRawRepository _repo;

    public AttendanceLogRawController(IAttendanceLogRawRepository repo)
    {
        _repo = repo;
    }

    // Helpers
    private string? GetRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }
    private int? GetEmployeeId() =>
        int.TryParse(User.FindFirstValue("EmployeeID"), out var id) ? id : null;
    private int? GetDeptId() =>
        int.TryParse(User.FindFirstValue("DepartmentID"), out var id) ? id : null;


    // ================================================================
    // GET ALL RAW LOGS
    // SuperAdmin + Admin → الجميع
    // UnitAdmin → يجب أن نجلب فقط موظفي قسمه
    // Employee → سجلاته فقط
    // ================================================================
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


    // ================================================================
    // GET BY LOG ID
    // ================================================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(long id)
    {
        var log = await _repo.GetById(id);
        if (log == null) return NotFound();

        var role = GetRole();

        if (role == "SuperAdmin" || role == "Admin")
            return Ok(log);

        if (role == "UnitAdmin")
            return log.EmployeeDeptID == GetDeptId()
                ? Ok(log)
                : Unauthorized("Not your department");

        if (role == "Employee")
            return log.EmployeeID == GetEmployeeId()
                ? Ok(log)
                : Unauthorized("Not your data");

        return Unauthorized();
    }


    // ================================================================
    // GET BY DEVICE USER (مثل بصمة معينة)
    // ================================================================
    [HttpGet("user/{deviceUserID}")]
    [Authorize]
    public async Task<IActionResult> GetByUser(string deviceUserID)
    {
        var role = GetRole();

        // SuperAdmin + Admin → كل شيء
        if (role == "SuperAdmin" || role == "Admin")
            return Ok(await _repo.GetByDeviceUserID(deviceUserID));

        // UnitAdmin → فقط موظفي قسمه
        if (role == "UnitAdmin")
            return Ok(await _repo.GetDeviceUserByDepartment(deviceUserID, GetDeptId()!.Value));

        // Employee → فقط بياناته
        if (role == "Employee")
        {
            var empId = GetEmployeeId();
            var logs = await _repo.GetByDeviceUserID(deviceUserID);

            return logs.Any(l => l.EmployeeID == empId)
                ? Ok(logs.Where(l => l.EmployeeID == empId))
                : Unauthorized("Not your logs");
        }

        return Unauthorized();
    }


    // ================================================================
    // GET UNPROCESSED ONLY
    // SuperAdmin + Admin فقط
    // ================================================================
    [HttpGet("unprocessed")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> GetUnprocessed()
    {
        return Ok(await _repo.GetUnprocessed());
    }


    // ================================================================
    // MARK PROCESSED
    // SuperAdmin + Admin فقط
    // ================================================================
    [HttpPut("mark/{id}")]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> MarkProcessed(long id)
    {
        return (await _repo.MarkProcessed(id)) > 0
            ? Ok("Marked as processed")
            : NotFound();
    }
}
