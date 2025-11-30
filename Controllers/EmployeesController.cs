using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models.DTOs.Employees;
using WebApplication3.Services;

[ApiController]
[Route("api/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly IEmployeeService _service;

    public EmployeesController(IEmployeeService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }
    [HttpGet("{id}/full")]
    public async Task<IActionResult> GetFullProfile(int id)
    {
        var result = await _service.GetFullProfile(id);
        if (result == null) return NotFound();

        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var emp = await _service.GetById(id);
        if (emp == null) return NotFound();
        return Ok(emp);
    }

    [HttpPost]
    public async Task<IActionResult> Create(EmployeeCreateDto dto)
    {
        var result = await _service.Create(dto);
        return result ? Ok("Employee Created") : BadRequest("Failed to create employee");
    }

    [HttpPut]
    public async Task<IActionResult> Update(EmployeeUpdateDto dto)
    {
        var result = await _service.Update(dto);
        return result ? Ok("Employee Updated") : NotFound("Employee not found");
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return result ? Ok("Employee Deleted") : NotFound("Employee not found");
    }
}
