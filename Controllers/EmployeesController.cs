using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Helpers;
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

    // 1) SuperAdmin + Admin + UnitAdmin + Employee (ولكن كل واحد حسب صلاحياته)
    [Authorize]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var role = UserContext.GetRole(User);
        var employeeId = UserContext.GetEmployeeID(User);
        var departmentId = UserContext.GetDepartmentID(User);

        // Employee → يشاهد نفسه فقط
        if (role == 2) // Employee
        {
            if (employeeId != id)
                return Forbid("You can view your own data only.");

            return Ok(await _repo.GetById(id));
        }

        // UnitAdmin → يشاهد موظفين قسمه فقط
        if (role == 3) // UnitAdmin
        {
            var employee = await _repo.GetById(id);
            if (employee == null) return NotFound();

            if (employee.DepartmentID != departmentId)
                return Forbid("You can see employees in your department only.");

            return Ok(employee);
        }

        // Admin → يرى الجميع لكن لا يعدّل
        if (role == 1) // Admin
        {
            return Ok(await _repo.GetById(id));
        }

        // SuperAdmin → كل الصلاحيات
        if (role == 0)
            return Ok(await _repo.GetById(id));

        return Forbid();
    }


    // 2) الحصول على كل الموظفين (Admin + SuperAdmin فقط)
    [Authorize(Policy = "AdminOrSuperAdmin")]
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _repo.GetAll());
    }


    // 3) UnitAdmin → كل موظفين قسمه فقط
    [Authorize(Policy = "UnitAdminOnly")]
    [HttpGet("department")]
    public async Task<IActionResult> GetMyDepartmentEmployees()
    {
        var deptID = UserContext.GetDepartmentID(User);

        if (deptID == null)
            return BadRequest("Department ID missing");

        return Ok(await _repo.GetByDepartment((int)deptID));
    }


    // 4) Employee → يشاهد ملفه الخاص فقط
    [Authorize(Policy = "EmployeeOnly")]
    [HttpGet("me")]
    public async Task<IActionResult> MyProfile()
    {
        var empId = UserContext.GetEmployeeID(User);

        if (empId == null)
            return Unauthorized("No EmployeeID in token");

        return Ok(await _repo.GetById((int)empId));
    }


    // 5) إضافة موظف (SuperAdmin فقط)
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPost]
    public async Task<IActionResult> Create(Employee model)
    {
        await _repo.Create(model);
        return Ok("Employee created");
    }


    // 6) تحديث موظف (SuperAdmin فقط)
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpPut]
    public async Task<IActionResult> Update(Employee model)
    {
        await _repo.Update(model);
        return Ok("Employee updated");
    }


    // 7) حذف موظف (SuperAdmin فقط)
    [Authorize(Policy = "SuperAdminOnly")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _repo.Delete(id);
        return Ok("Employee deleted");
    }
}
