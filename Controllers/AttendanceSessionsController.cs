using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.AttendanceSessions;
using WebApplication3.Repositories.AttendanceSessionRepository;

[ApiController]
[Route("api/[controller]")]
public class AttendanceSessionsController : ControllerBase
{
    private readonly IAttendanceSessionRepository _repo;

    public AttendanceSessionsController(IAttendanceSessionRepository repo)
    {
        _repo = repo;
    }

    // ============= Helper Methods =============
    private string? GetRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }
    private int? GetEmployeeId() =>
        int.TryParse(User.FindFirstValue("EmployeeID"), out var id) ? id : null;
    private int? GetDeptId() =>
        int.TryParse(User.FindFirstValue("DepartmentID"), out var id) ? id : null;

    // ============================================
    // GET ALL
    // SuperAdmin / Admin → كل السجلات
    // UnitAdmin → سجلات قسمه فقط
    // Employee → سجلاته فقط
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
    // GET BY ID
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
            return Unauthorized("This session is not from your department.");
        }

        if (role == "Employee")
        {
            if (item.EmployeeID == GetEmployeeId())
                return Ok(item);
            return Unauthorized("This session does not belong to you.");
        }

        return Unauthorized();
    }

    // ============================================
    // CREATE
    // Employee فقط → ينشئ جلسة لنفسه
    // ============================================
    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Create(AttendanceSessionCreateDto dto)
    {
        var empId = GetEmployeeId();
        if (empId == null)
            return Unauthorized();

        var model = new AttendanceSession
        {
            EmployeeID = empId.Value,
            CheckInTime = dto.CheckInTime,
            CheckOutTime = dto.CheckOutTime,
            WorkDate = dto.CheckInTime.Date,
            DurationMinutes = (int)(dto.CheckOutTime - dto.CheckInTime).TotalMinutes
        };

        return (await _repo.Create(model)) > 0
            ? Ok("Session created")
            : BadRequest();
    }

    // ============================================
    // UPDATE
    // SuperAdmin + Admin فقط
    // ============================================
    [HttpPut]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(AttendanceSessionUpdateDto dto)
    {
        var model = await _repo.GetById(dto.SessionID);
        if (model == null) return NotFound();

        model.EmployeeID = dto.EmployeeID;
        model.CheckInTime = dto.CheckInTime;
        model.CheckOutTime = dto.CheckOutTime;
        model.WorkDate = dto.CheckInTime.Date;
        model.DurationMinutes = (int)(dto.CheckOutTime - dto.CheckInTime).TotalMinutes;

        return (await _repo.Update(model)) > 0
            ? Ok("Session updated")
            : BadRequest();
    }

    // ============================================
    // DELETE
    // SuperAdmin فقط
    // ============================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(long id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("Session deleted")
            : NotFound();
    }
}
