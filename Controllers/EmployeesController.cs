using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models;
using WebApplication3.Repositories.EmployeeRepositiry;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeRepository _repo;

    public EmployeesController(IEmployeeRepository repo)
    {
        _repo = repo;
    }

    // ============================
    // Helper functions
    // ============================
    private string? GetRole() =>
        User.FindFirstValue(ClaimTypes.Role);   // SuperAdmin / Admin / UnitAdmin / Employee

    private int? GetEmployeeId() =>
        int.TryParse(User.FindFirstValue("EmployeeID"), out var id) ? id : null;

    private int? GetDeptId() =>
        int.TryParse(User.FindFirstValue("DepartmentID"), out var id) ? id : null;



    // ================================================================
    // GET EMPLOYEE BY ID
    // Employee → sees ONLY himself
    // UnitAdmin → sees ONLY his department
    // Admin + SuperAdmin → see all
    // ================================================================
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = GetRole();
        var empId = GetEmployeeId();
        var deptId = GetDeptId();

        var employee = await _repo.GetById(id);
        if (employee == null) return NotFound();


        // Employee → can view ONLY himself
        if (role == "Employee")
        {
            if (empId != id)
                return Forbid("You can only view your own profile.");

            return Ok(employee);
        }

        // UnitAdmin → can view ONLY employees in his department
        if (role == "UnitAdmin")
        {
            if (employee.DepartmentID != deptId)
                return Forbid("You can only view employees in your department.");

            return Ok(employee);
        }

        // Admin + SuperAdmin → full access
        if (role == "Admin" || role == "SuperAdmin")
            return Ok(employee);

        return Forbid();
    }



    // ================================================================
    // GET ALL EMPLOYEES
    // SuperAdmin + Admin ONLY
    // ================================================================
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }



    // ================================================================
    // GET EMPLOYEES IN CURRENT UNITADMIN DEPARTMENT
    // UnitAdmin ONLY
    // ================================================================
    [Authorize(Roles = "UnitAdmin")]
    [HttpGet("department")]
    public async Task<IActionResult> GetDepartmentEmployees()
    {
        var deptId = GetDeptId();
        if (deptId == null)
            return BadRequest("DepartmentID missing");

        return Ok(await _repo.GetByDepartment(deptId.Value));
    }



    // ================================================================
    // EMPLOYEE GET HIS OWN PROFILE
    // Employee ONLY
    // ================================================================
    [Authorize(Roles = "Employee")]
    [HttpGet("me")]
    public async Task<IActionResult> MyProfile()
    {
        var empId = GetEmployeeId();
        if (empId == null)
            return Unauthorized("EmployeeID is missing in token");

        return Ok(await _repo.GetById(empId.Value));
    }



    // ================================================================
    // CREATE EMPLOYEE
    // SuperAdmin ONLY
    // ================================================================
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(Employee model)
    {
        var res = await _repo.Create(model);
        return res > 0 ? Ok("Employee created") : BadRequest();
    }



    // ================================================================
    // UPDATE EMPLOYEE
    // SuperAdmin ONLY
    // ================================================================
    [Authorize(Roles = "SuperAdmin")]
    [HttpPut]
    public async Task<IActionResult> Update(Employee model)
    {
        var res = await _repo.Update(model);
        return res > 0 ? Ok("Employee updated") : NotFound();
    }



    // ================================================================
    // DELETE EMPLOYEE
    // SuperAdmin ONLY
    // ================================================================
    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var res = await _repo.Delete(id);
        return res > 0 ? Ok("Employee deleted") : NotFound();
    }
}
