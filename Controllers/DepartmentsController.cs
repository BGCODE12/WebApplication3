using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WebApplication3.Models.DTOs.Departments;
using WebApplication3.Services;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service)
    {
        _service = service;
    }

    // ======================
    // 🔥 Helpers (JWT Claims)
    // ======================
    private string? GetRole()
    {
        return User.FindFirstValue(ClaimTypes.Role);  // SuperAdmin / Admin / UnitAdmin / Employee
    }

    private int? GetDeptId()
    {
        return int.TryParse(User.FindFirstValue("DepartmentID"), out var id) ? id : null;
    }


    // =========================================
    // GET ALL DEPARTMENTS
    // Only Admin + SuperAdmin
    // =========================================
    [Authorize(Roles = "Admin,SuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }


    // =========================================
    // GET BY ID
    // SuperAdmin → كل شيء
    // Admin → كل شيء
    // UnitAdmin → قسمه فقط
    // Employee → ممنوع
    // =========================================
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var role = GetRole();
        var deptId = GetDeptId();

        if (role == "SuperAdmin" || role == "Admin")
        {
            var dept = await _service.GetById(id);
            return dept == null ? NotFound() : Ok(dept);
        }

        if (role == "UnitAdmin")
        {
            if (deptId != id)
                return Forbid("You can only access your own department.");

            var dept = await _service.GetById(id);
            return dept == null ? NotFound() : Ok(dept);
        }

        // Employee ممنوع
        return Forbid("Employees cannot view department data.");
    }


    // =========================================
    // CREATE — SuperAdmin Only
    // =========================================
    [Authorize(Roles = "SuperAdmin")]
    [HttpPost]
    public async Task<IActionResult> Create(DepartmentCreateDto dto)
    {
        var result = await _service.Create(dto);
        return result ? Ok("Department Created") : BadRequest();
    }


    // =========================================
    // UPDATE — SuperAdmin Only
    // =========================================
    [Authorize(Roles = "SuperAdmin")]
    [HttpPut]
    public async Task<IActionResult> Update(DepartmentUpdateDto dto)
    {
        var result = await _service.Update(dto);
        return result ? Ok("Department Updated") : NotFound();
    }


    // =========================================
    // DELETE — SuperAdmin Only
    // =========================================
    [Authorize(Roles = "SuperAdmin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return result ? Ok("Department Deleted") : NotFound();
    }
}
