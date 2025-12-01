using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Helpers;
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

    // ========== GET ALL DEPARTMENTS ==========
    // SuperAdmin + Admin Only
    [Authorize(Policy = "AdminOrSuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    // ========== GET BY ID ==========
    // SuperAdmin -> sees all
    // Admin -> sees all
    // UnitAdmin -> sees only his department
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var role = UserContext.GetRole(User);
        var deptId = UserContext.GetDepartmentID(User);

        // UnitAdmin → يشاهد قسمه فقط
        if (role == 3)
        {
            if (deptId != id)
                return Forbid("You can only view your own department.");

            var result = await _service.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        // Admin + SuperAdmin → يشاهدون كل الأقسام
        if (role == 0 || role == 1)
        {
            var result = await _service.GetById(id);
            return result == null ? NotFound() : Ok(result);
        }

        // Employee → ممنوع تمامًا
        return Forbid("You are not allowed to view department data.");
    }

    // ========== CREATE ==========
    // SuperAdmin Only
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create(DepartmentCreateDto dto)
    {
        var result = await _service.Create(dto);
        return result ? Ok("Department Created") : BadRequest();
    }

    // ========== UPDATE ==========
    // SuperAdmin Only
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPut]
    public async Task<IActionResult> Update(DepartmentUpdateDto dto)
    {
        var result = await _service.Update(dto);
        return result ? Ok("Department Updated") : NotFound();
    }

    // ========== DELETE ==========
    // SuperAdmin Only
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return result ? Ok("Department Deleted") : NotFound();
    }
}
