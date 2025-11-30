using WebApplication3.Services;
using Microsoft.AspNetCore.Mvc;
using WebApplication3.Models.DTOs.Departments;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _service;

    public DepartmentsController(IDepartmentService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        return Ok(await _service.GetAll());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(int id)
    {
        var dept = await _service.GetById(id);
        return dept == null ? NotFound() : Ok(dept);
    }

    [HttpPost]
    public async Task<IActionResult> Create(DepartmentCreateDto dto)
    {
        var result = await _service.Create(dto);
        return result ? Ok("Department Created") : BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> Update(DepartmentUpdateDto dto)
    {
        var result = await _service.Update(dto);
        return result ? Ok("Department Updated") : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return result ? Ok("Department Deleted") : NotFound();
    }
}
