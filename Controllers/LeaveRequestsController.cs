using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Models.DTOs.LeaveRequests;
using WebApplication3.Repositories.LeaveRequestRepository;

[ApiController]
[Route("api/[controller]")]
public class LeaveRequestsController : ControllerBase
{
    private readonly ILeaveRequestRepository _repo;

    public LeaveRequestsController(ILeaveRequestRepository repo)
    {
        _repo = repo;
    }

    private string? GetRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);
    }


    private int? GetEmployeeId() =>
        int.TryParse(User.FindFirstValue("EmployeeID"), out var id) ? id : null;
    private int? GetDeptId() =>
        int.TryParse(User.FindFirstValue("DepartmentID"), out var id) ? id : null;

    // ================================================================
    //  SUPERADMIN + ADMIN + UNITADMIN: GET ALL
    //  EMPLOYEE: ONLY HIS OWN REQUESTS
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
    // GET BY ID
    // Employee can ONLY view his own request
    // UnitAdmin can ONLY view requests from his department
    // ================================================================
    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> Get(int id)
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
            return Unauthorized("Not your request");
        }

        return Unauthorized();
    }

    // ================================================================
    // EMPLOYEE ONLY: CREATE REQUEST
    // ================================================================
    [HttpPost]
    [Authorize(Roles = "Employee")]
    public async Task<IActionResult> Create(LeaveRequestCreateDto dto)
    {
        var employeeId = GetEmployeeId();
        if (employeeId == null)
            return Unauthorized();

        var model = new LeaveRequest
        {
            EmployeeID = employeeId.Value,
            LeaveTypeID = dto.LeaveTypeID,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = "Pending",
            ApprovedByEmployeeID = null
        };

        return (await _repo.Create(model)) > 0
            ? Ok("Leave request created")
            : BadRequest();
    }

    // ================================================================
    // UPDATE → ADMIN + SUPERADMIN ONLY
    // ================================================================
    [HttpPut]
    [Authorize(Roles = "SuperAdmin,Admin")]
    public async Task<IActionResult> Update(LeaveRequestUpdateDto dto)
    {
        var model = await _repo.GetById(dto.LeaveRequestID);
        if (model == null) return NotFound();

        model.LeaveTypeID = dto.LeaveTypeID;
        model.StartDate = dto.StartDate;
        model.EndDate = dto.EndDate;
        model.Status = dto.Status;

        return (await _repo.Update(model)) > 0
            ? Ok("Leave request updated")
            : BadRequest();
    }

    // ================================================================
    // DELETE → SUPERADMIN ONLY
    // ================================================================
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IActionResult> Delete(int id)
    {
        return (await _repo.Delete(id)) > 0
            ? Ok("Leave request deleted")
            : NotFound();
    }

    // ================================================================
    // UNITADMIN + SUPERADMIN → APPROVE / REJECT
    // UnitAdmin: Only for his department
    // ================================================================
    [HttpPost("{id}/approve")]
    [Authorize(Roles = "UnitAdmin,SuperAdmin")]
    public async Task<IActionResult> Approve(int id)
    {
        var model = await _repo.GetById(id);
        if (model == null) return NotFound();

        var role = GetRole();

        if (role == "UnitAdmin" && model.EmployeeDeptID != GetDeptId())
            return Unauthorized("Request not in your department");

        model.Status = "Approved";
        model.ApprovedByEmployeeID = GetEmployeeId();

        return (await _repo.Update(model)) > 0
            ? Ok("Approved")
            : BadRequest();
    }

    [HttpPost("{id}/reject")]
    [Authorize(Roles = "UnitAdmin,SuperAdmin")]
    public async Task<IActionResult> Reject(int id)
    {
        var model = await _repo.GetById(id);
        if (model == null) return NotFound();

        var role = GetRole();

        if (role == "UnitAdmin" && model.EmployeeDeptID != GetDeptId())
            return Unauthorized("Request not in your department");

        model.Status = "Rejected";
        model.ApprovedByEmployeeID = GetEmployeeId();

        return (await _repo.Update(model)) > 0
            ? Ok("Rejected")
            : BadRequest();
    }
}
