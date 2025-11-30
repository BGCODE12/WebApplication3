using WebApplication3.Models.DTOs.Shift;
using WebApplication3.Services;
using Microsoft.AspNetCore.Mvc;
[ApiController]
[Route("api/[controller]")]
public class ShiftsController : ControllerBase
{
    private readonly IShiftService _service;

    public ShiftsController(IShiftService service)
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
        var shift = await _service.GetById(id);
        return shift == null ? NotFound() : Ok(shift);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ShiftCreateDto dto)
    {
        var result = await _service.Create(dto);
        return result ? Ok("Shift Created") : BadRequest();
    }

    [HttpPut]
    public async Task<IActionResult> Update(ShiftUpdateDto dto)
    {
        var result = await _service.Update(dto);
        return result ? Ok("Shift Updated") : NotFound();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _service.Delete(id);
        return result ? Ok("Shift Deleted") : NotFound();
    }
}
